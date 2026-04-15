import axios from 'axios';
import { useEffect, useState } from 'react';
import StarRating from './StarRating';

type Props = {
  productId: number;
};

type Review = {
  id: number;
  author: string;
  content: string;
  rating: number;
  createdAt: string;
};

type GetReviewsResponse = {
  summary: string | null;
  reviews: Review[];
};

const ReviewList = ({ productId }: Props) => {
  const [reviewData, setReviewData] = useState<GetReviewsResponse>();

  useEffect(() => {
    const fetchReviews = async () => {
      const response = await axios.get<GetReviewsResponse>(
        `/api/products/${productId}/reviews/summarize`
      );

      console.log('Fetched review data:', response.data);

      setReviewData(response.data);
    };

    fetchReviews();
  }, [productId]);

  return (
    <div className="flex flex-col gap-5">
      {reviewData?.reviews.map((review) => (
        <div key={review.id}>
          <div className="font-semibold">{review.author}</div>
          <div>
            <StarRating value={review.rating} />
          </div>
          <p>{review.content}</p>
          <p className="py-2">Created at: {review.createdAt}</p>
        </div>
      ))}
    </div>
  );
};

export default ReviewList;
