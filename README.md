# ExpressTest Monorepo Template

>A modern full-stack template repository for TypeScript projects with a React client (Vite, TailwindCSS) and an Express server, managed as a monorepo with Bun workspaces.

---

## Features

- **Monorepo**: Organize client and server in a single repository using Bun workspaces
- **Client**: React 19, Vite, TailwindCSS, TypeScript, ESLint, Prettier
- **Server**: Express 5, TypeScript, dotenv, Bun runtime
- **Fast install & scripts**: Powered by [Bun](https://bun.com)
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

This will start both the server and client concurrently.

- The client will be available at [http://localhost:5173](http://localhost:5173) and will proxy API requests to the server at [http://localhost:3000](http://localhost:3000).

---

## Project Structure

```
expresstest/
├── packages/
│   ├── client/   # React + Vite frontend
│   └── server/   # Express backend
├── package.json  # Bun workspace config
└── ...
```

### Client Highlights
- React 19, Vite, TailwindCSS, TypeScript
- Proxy setup for API requests (`/api/*` → server)
- Example component: `Button`
- Example API call: `/api/hello`

### Server Highlights
- Express 5, TypeScript, dotenv
- Example endpoint: `/api/hello`
- Reads environment variables from `.env`

---

## Customization

### Environment Variables
Create a `.env` file in `packages/server` for server-side secrets (e.g., `OPEN_API_KEY`).

### Linting & Formatting
- Run `bun run lint` and `bun run format` in each package for code quality.
- ESLint and Prettier are preconfigured.

### Building for Production

**Client:**
```bash
cd packages/client
bun run build
```

**Server:**
```bash
cd packages/server
bun run start
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
