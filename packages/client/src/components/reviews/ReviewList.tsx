import { useEffect, useState } from 'react';
import Skeleton from 'react-loading-skeleton';
import StarRating from './StarRating';
import { HiSparkles } from 'react-icons/hi2';
import { Button } from '../ui/button';
import { useMutation } from '@tanstack/react-query';
import type { Props, Review, SummaryApiResponse } from './reviewsApi';
import reviewsApi from './reviewsApi';

const ReviewList = ({ productId }: Props) => {
  const [reviewData, setReviewData] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isError, setIsError] = useState(false);

  const {
    mutate: handleSummarize,
    isPending: isSummarLoading,
    error: summaryError,
    data: summaryData,
  } = useMutation<SummaryApiResponse, Error>({
    mutationFn: () => reviewsApi.summarizeReviews(productId),
  });

  useEffect(() => {
    console.log(`useEffect triggered for product ID: ${productId}`);
    setIsLoading(true);

    const loadReviews = async () => {
      try {
        const data = await reviewsApi.fetchReviews(productId);
        console.log('Fetched reviews:', data);
        setReviewData(data);
        setIsError(false);
      } catch (error) {
        console.error('Error fetching reviews:', error);
        setIsError(true);
      } finally {
        setIsLoading(false);
      }
    };

    loadReviews();
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
      {isSummarLoading ? (
        <Skeleton width={100} height={20} className="mb-2" />
      ) : null}
      <div className="mb-5">
        {summaryData ? (
          <div>{summaryData.summary}</div>
        ) : (
          <Button
            className="rounded-full bg-black px-4 py-2 text-white shadow-sm hover:bg-neutral-800"
            onClick={() => handleSummarize()}
            disabled={isSummarLoading}
          >
            <HiSparkles className="mr-2" /> Summarize
          </Button>
        )}
        {summaryError && <p className="text-red-500">{summaryError.message}</p>}
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
