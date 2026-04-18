import { getPrisma } from '../db.ts';
import type { Express, Request, Response } from 'express';
import { reviewService } from '../services/review.service.ts';
import { productRepository } from '../repositories/product.repository.ts';

const reviewController = {
  getReviews: async (req: Request, res: Response) => {
    try {
      const prisma = getPrisma();
      const productId = Number(req.params.id);

      if (isNaN(productId)) {
        return res.status(400).json({ error: 'Invalid product ID' });
      }

      const product = await productRepository.getProduct(productId);

      if (!product) {
        return res.status(404).json({ error: 'Product not found' });
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

      const product = await productRepository.getProduct(productId);

      if (!product) {
        return res.status(404).json({ error: 'Product not found' });
      }

      //const reviews = await reviewService.getReviewsByProductId(productId);

      // if (reviews.length === 0) {
      //   return res.json({ summary: 'No reviews available for this product.' });
      // }

      // const reviews1 = await reviewService.getReviewsByProductId(productId);
      const summary = await reviewService.summarizeReviews(productId);

      res.json({
        summary: { summary },
      });
    } catch (error) {
      res.status(500).json({ error: 'Failed to summarize reviews' });
    }
  },
};

export default reviewController;
