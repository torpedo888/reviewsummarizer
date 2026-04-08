import { reviewRepository } from '../repositories/review.repositor';
import type { Review } from '../generated/prisma/client';

export const reviewService = {
  async getReviewsByProductId(productId: number): Promise<Review[]> {
    return reviewRepository.getReviewsByProductId(productId);
  },
};
