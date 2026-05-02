# ReviewSummarizer Monorepo

> A full-stack monorepo with a React client (Vite, TailwindCSS) and a .NET 8 Minimal API backend.

---

## Features

- **Monorepo**: Organize client and server in a single repository using Bun workspaces
- **Client**: React 19, Vite, TailwindCSS, TypeScript, ESLint, Prettier
- **Server**: ASP.NET Core 8 Minimal API, Dapper, MySQL
- **Workspace scripts**: Powered by [Bun](https://bun.com) for orchestration
- **Ready for customization**: Use as a template for your own projects

---

## Getting Started

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd expresstest
```

### 2. Install dependencies (all packages)

```bash
bun install
```

### 3. Development

#### Start both client and server together

From the root directory, run:

```bash
bun dev
```

This will start the .NET API server and client concurrently.

- The client will be available at [http://localhost:5173](http://localhost:5173) and will proxy API requests to the .NET server at [http://localhost:3000](http://localhost:3000).

---

## Project Structure

```
reviewsummarizer/
├── packages/
│   ├── client/   # React + Vite frontend
│   └── server-dotnet/ # Active .NET minimal API backend
├── package.json  # Bun workspace config
└── ...
```

### Client Highlights

- React 19, Vite, TailwindCSS, TypeScript
- Proxy setup for API requests (`/api/*` → server)
- Example component: `Button`
- Example API call: `/api/hello`

### Server Highlights

- ASP.NET Core 8 Minimal API
- Endpoints equivalent to the previous Express routes
- Uses `DATABASE_URL` and optional `OPEN_API_KEY`

---

## Customization

### Environment Variables

Set server-side environment variables before running the API:

- `DATABASE_URL`
- `OPEN_API_KEY` (optional)
- `OPENAI_MODEL` (optional)

### Linting & Formatting

- Run `bun run lint` and `bun run format` in each package for code quality.
- ESLint and Prettier are preconfigured.

### Building for Production

**Client:**

```bash
cd packages/client
bun run build
```

**Server (.NET):**

```bash
cd packages/server-dotnet
dotnet run --urls http://localhost:3000
```

---

## Template Usage

1. Click "Use this template" on GitHub or clone and reinitialize your own repo.
2. Update project name, author, and metadata in `package.json` files.
3. Replace example code in `src/` folders with your own logic.

---

## License

MIT — free to use and modify.

---

Created by Ferenc Batorligeti
