import type { Express, Request, Response } from 'express';
import { getPrisma } from './db.ts';
import reviewController from './controllers/review.controller.ts';

export function configureRoutes(app: Express, openApiKey?: string): void {
  app.get('/', (_req: Request, res: Response) => {
    res.send(
      openApiKey
        ? `Your Open API Key is: ${openApiKey}`
        : 'No Open API Key found in environment variables.'
    );
  });

  app.get('/api/hello', (_req: Request, res: Response) => {
    res.json({ message: 'Hello from the Express server' });
  });

  app.get('/api/products/:id/reviews', reviewController.getReviews);

  app.get(
    '/api/products/:id/reviews/summarize',
    reviewController.summarizeReviews
  );
}
