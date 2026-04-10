import { reviewRepository } from '../repositories/review.repositor';
import type { Review } from '../generated/prisma/client';
import { OpenAI } from 'openai/client';

type CreateResponseResult = {
  output_text: string;
};

const mockOpenAIClient = {
  responses: {
    async create(params: {
      model: string;
      input: string;
      temperature?: number;
      max_output_tokens?: number;
    }): Promise<CreateResponseResult> {
      const text = params.input.replace(/\s+/g, ' ').trim().slice(0, 240);

      return {
        output_text: `Mock summary (${params.model}): ${text}${text.length === 240 ? '...' : ''}`,
      };
    },
  },
};

const useMock = process.env.USE_MOCK === 'true' || !process.env.OPEN_API_KEY;

const client = useMock
  ? mockOpenAIClient
  : new OpenAI({ apiKey: process.env.OPEN_API_KEY });

export const reviewService = {
  async getReviewsByProductId(productId: number): Promise<Review[]> {
    return reviewRepository.getReviewsByProductId(productId);
  },

  async summarizeReviews(productId: number): Promise<string> {
    const reviews = await reviewRepository.getReviewsByProductId(productId);
    const joinedreviews = reviews.map((review) => review.content).join('');

    // prompt for OpenAI to summarize the reviews
    const prompt = `Summarize the following reviews into a short paragraph 
            highlighting the key points, both positive and negative:\n
            ${joinedreviews}`;
    console.log(prompt);

    //OpenAI client call to get the summary
    const response = await client.responses.create({
      model: '',
      input: prompt,
      temperature: 0.2,
      max_output_tokens: 500,
    });

    const summary = response.output_text;
    await reviewRepository
      .storeReviewSummary(productId, summary)
      .catch((err) => {
        console.error('Error storing review summary:', err);
      });

    // For demonstration, we'll return a placeholder summary
    return `Summary of reviews for product ${productId}: ${response.output_text}`;
  },
};
