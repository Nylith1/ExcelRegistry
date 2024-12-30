import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { UserDto } from "../assets/Models";

const ProtectedRoute = ({ children, requiredRole }: any) => {
  const [user, setUser] = useState<UserDto | null>(null);

  useEffect(() => {
    const savedUser = localStorage.getItem("user");
    if (savedUser) {
      const parsedUser = JSON.parse(savedUser);
      const userDto = {
        id: parsedUser.id,
        name: parsedUser.name,
        email: parsedUser.email,
        profilePhoto: parsedUser.profilePhoto,
        userRoleNames: parsedUser.userRoleNames || [],
      };
      setUser(userDto);
    }
  }, []);

  if (user === null) {
    return <div>Loading...</div>;
  }

  const token = localStorage.getItem("jwtToken");
  if (!token) {
    return <Navigate to="/" replace />;
  }

  const authorizedUser = user.userRoleNames.includes(requiredRole);
  return authorizedUser ? children : <Navigate to="/" />;
};
export default ProtectedRoute;
