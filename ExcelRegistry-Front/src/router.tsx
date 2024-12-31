import { createBrowserRouter } from "react-router-dom";
import ProtectedRoute from "./components/ProtectedRoute";
import WelcomePage from "./pages/WelcomePage";
import AdminToolkitPage from "./pages/AdminToolkitPage";
import RegentsPage from "./pages/RegentsPage";

export const router = createBrowserRouter([
  {
    children: [
      {
        path: "/",
        element: <WelcomePage />,
      },
      {
        path: "/regents",
        element: (
          <ProtectedRoute requiredRole="User">
            <RegentsPage />
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
