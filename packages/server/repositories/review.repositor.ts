import dayjs from 'dayjs';
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

  async storeReviewSummary(productId: number, summary: string): Promise<void> {
    const now = new Date();
    const expiresAt = dayjs().add(7, 'day').toDate();
    const data = {
      content: summary,
      expiresAt,
      generatedAt: now,
      productId,
    };

    const prisma = getPrisma();
    await prisma.summary.upsert({
      where: { productId },
      create: data,
      update: data,
    });
  },

  async getReviewSummary(
    productId: number
  ): Promise<{ content: string; expiresAt: Date } | null> {
    const prisma = getPrisma();
    const summary = await prisma.summary.findFirst({
      where: {
        AND: [{ productId }, { expiresAt: { gt: new Date() } }],
      },
    });
    return summary
      ? { content: summary.content, expiresAt: summary.expiresAt }
      : null;
  },
};
