import { env } from 'bun';
import express from 'express';
import type { Request, Response } from 'express';
import dotenv from 'dotenv';

dotenv.config();

const app = express();
const port = process.env.PORT || 3000;

app.get('/', (req: Request, res: Response) => {
  res.send(
    env.OPEN_API_KEY
      ? `Your Open API Key is: ${env.OPEN_API_KEY}`
      : 'No Open API Key found in environment variables.'
  );
});

app.get('/api/hello', (req: Request, res: Response) => {
  res.json({ message: 'Hello from the Express server' });
});

app.listen(port, () => {
  console.log(`Server is running at http://localhost:${port}`);
});
