import React from "react";
import ReactDOM from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import "./index.css";
import { router } from "./router";
import { TokenChecker } from "./components/TokenChecker";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <TokenChecker>
      <RouterProvider router={router} />
    </TokenChecker>
  </React.StrictMode>
);
