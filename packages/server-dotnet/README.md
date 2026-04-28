# server-dotnet

ASP.NET Core 8 Minimal API backend replacing the previous Express implementation.

## Environment variables

- `DATABASE_URL`: MySQL connection string (same value the Prisma backend used)
- `OPEN_API_KEY`: optional; if missing, summary endpoint uses a mock summary
- `OPENAI_MODEL`: optional; defaults to `gpt-4.1-mini`

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
