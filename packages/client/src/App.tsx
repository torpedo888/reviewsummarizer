import { useEffect, useState } from 'react';
import './App.css';
import { Button } from './components/ui/button';

function App() {
  const [message, setMessage] = useState('');

  useEffect(() => {
    fetch('/api/hello')
      .then((response) => response.json())
      .then((data) => setMessage(data.message))
      .catch((error) => console.error('Error fetching message:', error));
  }, []);

  return (
    <div className="font-bold p-4">
      <p>{message}</p>
      <Button className="mt-4">Click Me now!</Button>
    </div>
  );
}

export default App;
