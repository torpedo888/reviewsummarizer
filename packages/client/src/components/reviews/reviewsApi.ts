import axios from 'axios';

export type Props = {
  productId: number;
};

export type Review = {
  id: number;
  author: string;
  content: string;
  rating: number;
  createdAt: string;
};

export type SummaryApiResponse = {
  summary: string | null;
};

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim();
const normalizedApiBaseUrl = apiBaseUrl?.replace(/\/+$/, '');

const apiClient = axios.create({
  // In local dev, leave this undefined and use Vite proxy for /api.
  baseURL: normalizedApiBaseUrl || undefined,
});

const reviewsApi = {
  async fetchReviews(productId: number) {
    const { data } = await apiClient.get<Review[]>(
      `/api/products/${productId}/reviews`
    );
    return data;
  },

  async summarizeReviews(productId: number) {
    const { data } = await apiClient.get<{ summary: SummaryApiResponse }>(
      `/api/products/${productId}/reviews/summarize`
    );
    return data.summary;
  },
};

export default reviewsApi;
