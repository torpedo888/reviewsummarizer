import { getPrisma } from '../db';

const prisma = getPrisma();
export const productRepository = {
  getProduct(productId: number) {
    return prisma.product.findUnique({
      where: { id: productId },
    });
  },
};
