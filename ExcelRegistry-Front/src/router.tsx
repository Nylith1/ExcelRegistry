import { createBrowserRouter } from "react-router-dom";
import DashboardPage from "./pages/DashboardPage";
import ProtectedRoute from "./components/ProtectedRoute";
import WelcomePage from "./pages/WelcomePage";
import AdminToolkitPage from "./pages/AdminToolkitPage";

export const router = createBrowserRouter([
  {
    children: [
      {
        path: "/",
        element: <WelcomePage />,
      },
      {
        path: "/dashboard",
        element: (
          <ProtectedRoute requiredRole="User">
            <DashboardPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "/adminToolkit",
        element: (
          <ProtectedRoute requiredRole="Admin">
            <AdminToolkitPage />
          </ProtectedRoute>
        ),
      },
    ],
  },
]);
