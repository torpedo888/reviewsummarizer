import { getPrisma } from '../db.ts';
import type { Express, Request, Response } from 'express';
import { reviewService } from '../services/review.service.ts';

const reviewController = {
  getReviews: async (req: Request, res: Response) => {
    try {
      const prisma = getPrisma();
      const productId = Number(req.params.id);

      if (isNaN(productId)) {
        return res.status(400).json({ error: 'Invalid product ID' });
      }

      const reviews = await reviewService.getReviewsByProductId(productId);

      res.json(reviews);
    } catch (error) {
      res.status(500).json({ error: 'Failed to fetch reviews' });
    }
  },

  summarizeReviews: async (req: Request, res: Response) => {
    try {
      const productId = Number(req.params.id);

      if (isNaN(productId)) {
        return res.status(400).json({ error: 'Invalid product ID' });
      }

      const summary = await reviewService.summarizeReviews(productId);

      res.json({ summary });
    } catch (error) {
      res.status(500).json({ error: 'Failed to summarize reviews' });
    }
  },
};

export default reviewController;
