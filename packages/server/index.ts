import { env } from 'bun';
import express from 'express';
import dotenv from 'dotenv';
import { configureRoutes } from './routes.ts';

dotenv.config();

const app = express();
const port = process.env.PORT || 3000;

configureRoutes(app, env.OPEN_API_KEY);

app.listen(port, () => {
  console.log(`Server is running at http://localhost:${port}`);
});
