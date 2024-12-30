import { useState, useEffect } from "react";

interface CountdownTimerProps {
  countdownSeconds: number;
  text: string;
}

const CountdownTimer = ({ countdownSeconds, text }: CountdownTimerProps) => {
  const [timeLeft, setTimeLeft] = useState(countdownSeconds);

  useEffect(() => {
    const interval = setInterval(() => {
      setTimeLeft((prevTime) => (prevTime > 0 ? prevTime - 1 : 0));
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  const formatTime = (time: any) => {
    const minutes = Math.floor(time / 60);
    const seconds = time % 60;
    return `${minutes.toString().padStart(2, "0")}:${seconds
      .toString()
      .padStart(2, "0")}`;
  };

  return (
    <div
      className={`${
        countdownSeconds < 0 ? "hidden" : ""
      } flex flex-col items-center w-40 space-y-2`}
    >
      {`${text} 
      ${formatTime(timeLeft)}`}
    </div>
  );
};

export default CountdownTimer;
