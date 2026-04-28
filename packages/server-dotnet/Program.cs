using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Dapper;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IDbConnectionFactory>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnectionFactory(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:5173");
    });
});

var app = builder.Build();
app.UseCors();

app.MapGet("/", () =>
{
    var openApiKey = app.Configuration["OpenAI:ApiKey"];
    return !string.IsNullOrWhiteSpace(openApiKey)
        ? Results.Text($"Your Open API Key is: {openApiKey}")
    : Results.Text("No Open API Key found in configuration.");
});

app.MapGet("/api/hello", () => Results.Json(new { message = "Hello from the .NET Minimal API server" }));

app.MapGet("/api/products/{id:int}/reviews", async (int id, IDbConnectionFactory factory) =>
{
    await using var connection = await factory.CreateOpenConnectionAsync();

    var productExists = await connection.ExecuteScalarAsync<int>(
        "SELECT COUNT(1) FROM products WHERE id = @id",
        new { id });

    if (productExists == 0)
    {
        return Results.NotFound(new { error = "Product not found" });
    }

    var reviews = await connection.QueryAsync<ReviewDto>(
        @"SELECT
			id AS Id,
			author AS Author,
			COALESCE(content, '') AS Content,
            CAST(rating AS SIGNED) AS Rating,
			createdAt AS CreatedAt
		  FROM reviews
		  WHERE productId = @id
		  ORDER BY createdAt DESC",
        new { id });

    return Results.Ok(reviews);
});

app.MapGet("/api/products/{id:int}/reviews/summarize", async (
    int id,
    IDbConnectionFactory factory,
    IHttpClientFactory httpClientFactory,
    ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("SummaryEndpoint");
    await using var connection = await factory.CreateOpenConnectionAsync();

    var productExists = await connection.ExecuteScalarAsync<int>(
        "SELECT COUNT(1) FROM products WHERE id = @id",
        new { id });

    if (productExists == 0)
    {
        return Results.NotFound(new { error = "Product not found" });
    }

    var cachedSummary = await connection.QueryFirstOrDefaultAsync<SummaryRow>(
        @"SELECT content AS Content, expiresAt AS ExpiresAt
		  FROM summaries
		  WHERE productId = @id AND expiresAt > UTC_TIMESTAMP()
		  LIMIT 1",
        new { id });

    if (cachedSummary is not null)
    {
        var cached = $"Summary of reviews for product {id}: {cachedSummary.Content}";
        return Results.Ok(new { summary = new { summary = cached } });
    }

    var reviews = await connection.QueryAsync<string>(
        @"SELECT COALESCE(content, '')
		  FROM reviews
		  WHERE productId = @id
		  ORDER BY createdAt DESC",
        new { id });

    var joinedReviews = string.Join(" ", reviews).Trim();

    if (string.IsNullOrWhiteSpace(joinedReviews))
    {
        return Results.Ok(new { summary = new { summary = "No reviews available for this product." } });
    }

    var prompt =
        "Summarize the following reviews into a short paragraph highlighting key positive and negative points:\n" +
        joinedReviews;

    var summaryText = await GetSummaryAsync(prompt, httpClientFactory, logger, app.Configuration);
    var fullSummary = $"Summary of reviews for product {id}: {summaryText}";

    await connection.ExecuteAsync(
        @"INSERT INTO summaries (productId, content, generatedAt, expiresAt)
		  VALUES (@id, @content, UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 7 DAY))
		  ON DUPLICATE KEY UPDATE
			content = VALUES(content),
			generatedAt = VALUES(generatedAt),
			expiresAt = VALUES(expiresAt)",
        new { id, content = summaryText });

    return Results.Ok(new { summary = new { summary = fullSummary } });
});

app.Run();

static async Task<string> GetSummaryAsync(
    string prompt,
    IHttpClientFactory httpClientFactory,
    ILogger logger,
    IConfiguration configuration)
{
    var apiKey = configuration["OpenAI:ApiKey"];
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        var sample = prompt.Replace('\n', ' ').Trim();
        return $"Mock summary: {sample[..Math.Min(sample.Length, 240)]}";
    }

    var model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";
    var payload = JsonSerializer.Serialize(new
    {
        model,
        input = prompt,
        temperature = 0.2,
        max_output_tokens = 500,
    });

    using var client = httpClientFactory.CreateClient();
    using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

    using var response = await client.SendAsync(request);
    var body = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
    {
        logger.LogWarning("OpenAI response failed with status code {StatusCode}: {Body}", response.StatusCode, body);
        throw new InvalidOperationException("Failed to generate summary from OpenAI.");
    }

    using var document = JsonDocument.Parse(body);
    if (document.RootElement.TryGetProperty("output_text", out var outputTextElement))
    {
        var outputText = outputTextElement.GetString();
        if (!string.IsNullOrWhiteSpace(outputText))
        {
            return outputText;
        }
    }

    throw new InvalidOperationException("OpenAI did not return output_text.");
}

public interface IDbConnectionFactory
{
    Task<MySqlConnection> CreateOpenConnectionAsync();
}

public sealed class MySqlConnectionFactory(string? connectionString) : IDbConnectionFactory
{
    public async Task<MySqlConnection> CreateOpenConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not set.");
        }

        var normalizedConnectionString = NormalizeConnectionString(connectionString);
        var connection = new MySqlConnection(normalizedConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    private static string NormalizeConnectionString(string rawConnectionString)
    {
        if (!Uri.TryCreate(rawConnectionString, UriKind.Absolute, out var uri) ||
            !string.Equals(uri.Scheme, "mysql", StringComparison.OrdinalIgnoreCase))
        {
            return rawConnectionString;
        }

        var database = uri.AbsolutePath.Trim('/');
        if (string.IsNullOrWhiteSpace(database))
        {
            throw new InvalidOperationException("MySQL URL must include a database name in the path.");
        }

        var builder = new MySqlConnectionStringBuilder
        {
            Server = uri.Host,
            Port = (uint)(uri.IsDefaultPort ? 3306 : uri.Port),
            Database = database,
            UserID = Uri.UnescapeDataString(uri.UserInfo.Split(':')[0]),
        };

        var userInfoParts = uri.UserInfo.Split(':', 2);
        if (userInfoParts.Length == 2)
        {
            builder.Password = Uri.UnescapeDataString(userInfoParts[1]);
        }

        return builder.ConnectionString;
    }
}

public sealed class ReviewDto
{
    public int Id { get; init; }
    public string Author { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public long Rating { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class SummaryRow
{
    public string Content { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
