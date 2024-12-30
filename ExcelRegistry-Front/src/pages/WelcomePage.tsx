import { GoogleLogin, GoogleOAuthProvider } from "@react-oauth/google";
import axios from "axios";

function WelcomePage() {
  const handleSuccess = async (credentialResponse: any) => {
    try {
      await axios
        .post(`${BASE_URL}Users/AuthenticateWithGoogle`, {
          token: credentialResponse.credential,
        })
        .then((response) => {
          localStorage.setItem("user", JSON.stringify(response.data));
          localStorage.setItem("jwtToken", response.data.jwtToken);
          window.location.href = "/dashboard";
        });
    } catch (error) {
      alert(
        "Prisijungimas nepavyko. Bandykite dar kartą vėliau. Jei vistiek nepavyksta susisiekite su administratoriumi"
      );
    }
  };

  const CLIENT_ID = import.meta.env.VITE_CLIENT_ID;
  const BASE_URL = import.meta.env.VITE_BASE_URL;

  return (
    <div className="flex items-center justify-center min-h-screen bg-blue-100 dark:bg-gray-900">
      <div className="text-center">
        <h1 className="mb-4 text-4xl font-extrabold leading-none tracking-tight text-gray-900 md:text-5xl lg:text-6xl dark:text-white">
          Sveiki atvykę į Regentų Registrą
        </h1>
        <p className="mb-6 text-lg font-normal text-gray-500 lg:text-xl sm:px-16 xl:px-48 dark:text-gray-400">
          Mūsų platforma suteikia paprastą ir efektyvų būdą valdyti ir
          organizuoti regentų informaciją.
        </p>
        <a className="inline-flex items-center justify-center px-5 py-3 text-base font-medium text-center">
          <GoogleOAuthProvider clientId={CLIENT_ID}>
            <div>
              <GoogleLogin theme="filled_blue" onSuccess={handleSuccess} />
            </div>
          </GoogleOAuthProvider>
        </a>
      </div>
    </div>
  );
}

export default WelcomePage;
