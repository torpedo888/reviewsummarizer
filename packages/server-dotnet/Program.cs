using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddDbContext<ReviewSummarizerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var configuredOrigins =
    builder.Configuration["Cors:AllowedOrigins"]
    ?? builder.Configuration["CORS_ALLOWED_ORIGINS"]
    ?? "http://localhost:5173";
var allowedOrigins = configuredOrigins
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod();

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }
        else
        {
            policy.AllowAnyOrigin();
        }
    });
});

var app = builder.Build();
app.UseCors();

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReviewSummarizerDbContext>();
    await SeedData.InitializeAsync(db);
}

app.MapGet("/", () =>
{
    return Results.Ok(new { status = "ok" });
});

app.MapGet("/api/hello", () => Results.Json(new { message = "Hello from the .NET Minimal API server" }));

app.MapGet("/api/products/{id:int}/reviews", async (int id, ReviewSummarizerDbContext db) =>
{
    var productExists = await db.Products.AnyAsync(p => p.Id == id);
    if (!productExists)
        return Results.NotFound(new { error = "Product not found" });

    var reviews = await db.Reviews
        .Where(r => r.ProductId == id)
        .OrderByDescending(r => r.CreatedAt)
        .Select(r => new ReviewDto
        {
            Id = r.Id,
            Author = r.Author,
            Content = r.Content ?? string.Empty,
            Rating = r.Rating,
            CreatedAt = r.CreatedAt,
        })
        .ToListAsync();

    return Results.Ok(reviews);
});

app.MapGet("/api/products", async (
    ReviewSummarizerDbContext db
    ) =>
{
    var products = await db.Products.ToListAsync();

    return Results.Ok(products);
});

app.MapGet("/api/products/{id:int}/reviews/summarize", async (
    int id,
    ReviewSummarizerDbContext db,
    IHttpClientFactory httpClientFactory,
    ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("SummaryEndpoint");

    var productExists = await db.Products.AnyAsync(p => p.Id == id);
    if (!productExists)
        return Results.NotFound(new { error = "Product not found" });

    var cachedSummary = await db.Summaries
        .FirstOrDefaultAsync(s => s.ProductId == id && s.ExpiresAt > DateTime.UtcNow);

    if (cachedSummary is not null)
    {
        var cached = $"Summary of reviews for product {id}: {cachedSummary.Content}";
        return Results.Ok(new { summary = new { summary = cached } });
    }

    var reviewContents = await db.Reviews
        .Where(r => r.ProductId == id)
        .OrderByDescending(r => r.CreatedAt)
        .Select(r => r.Content ?? string.Empty)
        .ToListAsync();

    var joinedReviews = string.Join(" ", reviewContents).Trim();

    if (string.IsNullOrWhiteSpace(joinedReviews))
        return Results.Ok(new { summary = new { summary = "No reviews available for this product." } });

    var prompt =
        "Summarize the following reviews into a short paragraph highlighting key positive and negative points:\n" +
        joinedReviews;

    var summaryText = await GetSummaryAsync(prompt, httpClientFactory, logger, app.Configuration);
    var fullSummary = $"Summary of reviews for product {id}: {summaryText}";

    var now = DateTime.UtcNow;
    var existing = await db.Summaries.FirstOrDefaultAsync(s => s.ProductId == id);
    if (existing is null)
    {
        db.Summaries.Add(new Summary
        {
            ProductId = id,
            Content = summaryText,
            GeneratedAt = now,
            ExpiresAt = now.AddDays(7),
        });
    }
    else
    {
        existing.Content = summaryText;
        existing.GeneratedAt = now;
        existing.ExpiresAt = now.AddDays(7);
    }
    await db.SaveChangesAsync();

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

// ── Entity models ─────────────────────────────────────────────────────────────

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Price { get; set; }
    public ICollection<Review> Reviews { get; set; } = [];
    public Summary? Summary { get; set; }
}

public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Author { get; set; } = string.Empty;
    public short Rating { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Product Product { get; set; } = null!;
}

public class Summary
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Product Product { get; set; } = null!;
}

// ── DbContext ──────────────────────────────────────────────────────────────────

public class ReviewSummarizerDbContext(DbContextOptions<ReviewSummarizerDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Summary> Summaries => Set<Summary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().ToTable("products");

        modelBuilder.Entity<Review>(e =>
        {
            e.ToTable("reviews");
            e.HasOne(r => r.Product)
             .WithMany(p => p.Reviews)
             .HasForeignKey(r => r.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Summary>(e =>
        {
            e.ToTable("summaries");
            e.HasIndex(s => s.ProductId).IsUnique();
            e.HasOne(s => s.Product)
             .WithOne(p => p.Summary)
             .HasForeignKey<Summary>(s => s.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

// ── Response DTO ───────────────────────────────────────────────────────────────

public sealed class ReviewDto
{
    public int Id { get; init; }
    public string Author { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public short Rating { get; init; }
    public DateTime CreatedAt { get; init; }
}
