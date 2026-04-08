import { getPrisma } from '../db';
import type { Review } from '../generated/prisma/client';

export const reviewRepository = {
  async getReviewsByProductId(productId: number): Promise<Review[]> {
    const prisma = getPrisma();
    return prisma.review.findMany({
      where: { productId },
      orderBy: { createdAt: 'desc' },
    });
  },
};
