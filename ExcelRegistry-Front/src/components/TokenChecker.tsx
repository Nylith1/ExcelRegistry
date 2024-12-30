import { useEffect } from "react";
import { ReactNodeProps } from "../assets/Models";
import axios from "axios";

const TokenChecker = ({ children }: ReactNodeProps) => {
  useEffect(() => {
    const intervalId = setInterval(() => {
      if (window.location.pathname === "/") {
        return;
      }

      const token = localStorage.getItem("jwtToken");
      if (!token) {
        window.location.href = "/";
        return;
      }

      const decodedToken = JSON.parse(atob(token.split(".")[1]));
      const isExpired = decodedToken.exp * 1000 < Date.now();
      if (isExpired) {
        window.location.href = "/";
      } else {
        const currentTime = Date.now();
        const expirationTime = decodedToken.exp * 1000;
        const timeRemaining = expirationTime - currentTime;
        const threshold = 5 * 60 * 1000;
        if (timeRemaining < threshold) {
          refreshToken();
        }
      }
    }, 10000);
    return () => clearInterval(intervalId);
  }, []);

  const refreshToken = () => {
    try {
      axios
        .get(`${BASE_URL}Users/GetRefreshedJwtToken`, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
          },
        })
        .then((response) => {
          localStorage.setItem("jwtToken", response.data.jwtToken);
        });
    } catch (error) {
      alert(
        "Prisijungimas nepavyko. Bandykite dar kartą vėliau. Jei vistiek nepavyksta susisiekite su administratoriumi"
      );
      window.location.href = "/";
    }
  };

  const BASE_URL = import.meta.env.VITE_BASE_URL;

  return <>{children}</>;
};

export { TokenChecker };
