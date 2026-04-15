import axios from 'axios';
import { useEffect, useState } from 'react';
import Skeleton from 'react-loading-skeleton';
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

  const [isLoading, setIsLoading] = useState(true);

  const [isError, setIsError] = useState(false);

  useEffect(() => {
    setIsLoading(true);

    // method fetching review data from the backend
    const fetchReviews = async () => {
      const response = await axios.get<GetReviewsResponse>(
        `/api/products/${productId}/reviews/summarize`
      );
      console.log('Fetched review data:', response.data);

      setReviewData(response.data);
    };

    try {
      setIsLoading(true);
      fetchReviews();
      setIsLoading(false);
    } catch (error) {
      console.error('Error fetching review data:', error);
      setIsError(true);
      return;
    } finally {
      setIsLoading(false);
    }
  }, [productId]);

  if (isError) {
    return (
      <div className="text-red-500">
        Error loading reviews. Please try again later.
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex flex-col gap-5">
        {[1, 2, 3].map((index) => (
          <div key={index}>
            <Skeleton width={100} height={20} className="mb-2" />
            <Skeleton width={80} height={20} className="mb-2" />
            <Skeleton count={3} />
          </div>
        ))}
      </div>
    );
  }

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
