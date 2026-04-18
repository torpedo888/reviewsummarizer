import axios from 'axios';
import { useEffect, useState } from 'react';
import Skeleton from 'react-loading-skeleton';
import StarRating from './StarRating';
import { HiSparkles } from 'react-icons/hi2';
import { Button } from '../ui/button';

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

type SummaryApiResponse = {
  summary: string | null;
};

const ReviewList = ({ productId }: Props) => {
  const [reviewData, setReviewData] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);
  const [summaryData, setSummaryData] = useState<SummaryApiResponse | null>(
    null
  );
  const [isSummarizing, setIsSummarizing] = useState(false);

  // method fetching review data from the backend
  const fetchReviews = async () => {
    try {
      console.log(`Fetching reviews for product ID: ${productId}`);
      const response = await axios.get<Review[]>(
        `/api/products/${productId}/reviews`
      );

      console.log('response status', response.status);
      console.log('response data', response.data);

      setReviewData(response.data);
    } catch (error) {
      console.error('Error fetching review data:', error);
      setIsError(true);
      return;
    } finally {
      setIsLoading(false);
    }
  };

  const fetchSummary = async () => {
    try {
      setIsSummarizing(true);
      console.log(`Fetching summary for product ID: ${productId}`);
      const response = await axios.get<{ summary: SummaryApiResponse }>(
        `/api/products/${productId}/reviews/summarize`
      );

      console.log('response status', response.status);
      console.log('response data', response.data);

      setSummaryData(response.data.summary);
    } catch (error) {
      console.error('Error fetching summary:', error);
      setIsError(true);
    } finally {
      setIsSummarizing(false);
    }
  };

  useEffect(() => {
    console.log(`useEffect triggered for product ID: ${productId}`);
    setIsLoading(true);
    fetchReviews();
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
    <div>
      {isSummarizing ? (
        <Skeleton width={100} height={20} className="mb-2" />
      ) : null}
      <div className="mb-5">
        {summaryData ? (
          <div>{summaryData.summary}</div>
        ) : (
          <Button
            className="rounded-full bg-black px-4 py-2 text-white shadow-sm hover:bg-neutral-800"
            onClick={fetchSummary}
          >
            <HiSparkles className="mr-2" /> Summarize
          </Button>
        )}
      </div>
      <div className="flex flex-col gap-5">
        {reviewData.map((review) => (
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
    </div>
  );
};

export default ReviewList;
