import axios from 'axios';
import React from 'react';

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

const reviewsApi = {
  async fetchReviews(productId: number) {
    const { data } = await axios.get<Review[]>(
      `/api/products/${productId}/reviews`
    );
    return data;
  },

  async summarizeReviews(productId: number) {
    const { data } = await axios.get<{ summary: SummaryApiResponse }>(
      `/api/products/${productId}/reviews/summarize`
    );
    return data.summary;
  },
};

export default reviewsApi;
