import { FaRegStar, FaStar } from 'react-icons/fa';

type Props = {
  value: number;
};

const StarRating = ({ value }: Props) => {
  const placeholders = [1, 2, 3, 4, 5];

  return (
    <div className="flex gap-1">
      {placeholders.map((placeholder) =>
        placeholder <= value ? (
          <FaStar key={placeholder} className="text-yellow-500" />
        ) : (
          <FaRegStar key={placeholder} className="text-gray-400" />
        )
      )}
    </div>
  );
};

export default StarRating;
