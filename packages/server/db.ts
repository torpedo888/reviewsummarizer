import { PrismaClient } from './generated/prisma/client.ts';
import { PrismaMariaDb } from '@prisma/adapter-mariadb';
import 'dotenv/config';

let prismaInstance: PrismaClient | null = null;

export function getPrisma(): PrismaClient {
  if (!prismaInstance) {
    const databaseUrl = process.env.DATABASE_URL;

    if (!databaseUrl) {
      throw new Error('DATABASE_URL is not set');
    }

    const adapter = new PrismaMariaDb(databaseUrl);
    prismaInstance = new PrismaClient({ adapter });
  }

  return prismaInstance;
}
