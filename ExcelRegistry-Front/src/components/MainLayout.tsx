import { useEffect, useState } from "react";
import { UserDto, ReactNodeProps } from "../assets/Models";

function MainLayout({ children }: ReactNodeProps) {
  const [user, setUser] = useState<UserDto | null>(null);

  function handleLogout() {
    localStorage.removeItem("user");
    localStorage.removeItem("jwtToken");
    window.location.href = "/";
    setUser(null);
  }

  function handleSettingsClick() {
    window.location.href = "/adminToolkit";
  }

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

  return (
    <div className="h-screen bg-gray-000 dark:bg-gray-800">
      <nav className="border-gray-200 dark:bg-gray-900 bg-blue-100">
        <div className="flex flex-wrap items-center justify-between mx-auto p-4">
          <a
            href="/regents"
            className="flex items-center space-x-3 rtl:space-x-reverse"
          >
            <img
              src="https://flowbite.com/docs/images/logo.svg"
              className="h-8"
              alt="Flowbite Logo"
            />
            <span className="self-center text-2xl font-semibold whitespace-nowrap dark:text-white">
              Regentų registras
            </span>
          </a>
          <div
            className={`${
              user == null ? "hidden" : ""
            } items-center md:order-2 space-x-3 md:space-x-0 rtl:space-x-reverse`}
          >
            <button
              type="button"
              className="flex text-sm bg-gray-800 rounded-full md:me-0 focus:ring-4 focus:ring-gray-300 dark:focus:ring-gray-600"
              id="user-menu-button"
              aria-expanded="false"
              data-dropdown-toggle="user-dropdown"
              data-dropdown-placement="bottom"
            >
              <span className="sr-only">Open user menu</span>
              <img
                className="w-8 h-8 rounded-full"
                src={user?.profilePhoto}
                alt="user photo"
              ></img>
            </button>

            <div
              className="z-50 hidden my-4 text-base list-none bg-white divide-y divide-gray-100 rounded-lg shadow dark:bg-gray-700 dark:divide-gray-600"
              id="user-dropdown"
            >
              <div className="px-4 py-3">
                <span className="block text-sm text-gray-900 dark:text-white">
                  {user?.name}
                </span>
                <span className="block text-sm  text-gray-500 truncate dark:text-gray-400">
                  {user?.email}
                </span>
              </div>
              <ul className="py-2" aria-labelledby="user-menu-button">
                {user?.userRoleNames.some((role) => role === "Admin") && (
                  <li>
                    <a
                      onClick={handleSettingsClick}
                      className="block cursor-pointer px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 dark:text-gray-200 dark:hover:text-white"
                    >
                      Administratoriaus nustatymai
                    </a>
                  </li>
                )}
                <li>
                  <a
                    onClick={handleLogout}
                    className="block cursor-pointer px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 dark:text-gray-200 dark:hover:text-white"
                  >
                    Atsijungti
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </nav>
      <div>{children}</div>
    </div>
  );
}

export default MainLayout;
