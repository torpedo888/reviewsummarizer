# server-dotnet

ASP.NET Core 8 Minimal API backend replacing the previous Express implementation.

## Environment variables

- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `OpenAI__ApiKey`: optional; if missing, summary endpoint uses a mock summary
- `OpenAI__Model`: optional; defaults to `gpt-4o-mini`
- `CORS_ALLOWED_ORIGINS`: comma-separated origins (for example: `https://your-frontend.up.railway.app`)

## Run

```bash
dotnet watch run --urls http://localhost:3000
```

## Endpoints

- `GET /`
- `GET /api/hello`
- `GET /api/products/{id}/reviews`
- `GET /api/products/{id}/reviews/summarize`

The summarize endpoint caches generated summaries in `summaries` for 7 days, matching the behavior of the prior backend.
