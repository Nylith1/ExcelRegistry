import { Table } from "flowbite-react";
import MainLayout from "../components/MainLayout";
import { useEffect, useState } from "react";
import axios from "axios";
import { UserDto } from "../assets/Models";
import { LoadinBackdrop } from "../components/LoadinBackdrop";

function AdminToolkitPage() {
  const [shouldFetch, setShouldFetch] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [users, setUsers] = useState<UserDto[]>([]);
  const [currentUser, setCurrentUser] = useState<UserDto | null>(null);

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");

  useEffect(() => {
    if (shouldFetch) {
      const savedUser = localStorage.getItem("user");
      if (savedUser) {
        setCurrentUser(JSON.parse(savedUser));
      }

      fetchUsers();
      setShouldFetch(false);
      console.log(users);
    }
  }, [shouldFetch]);

  const fetchUsers = async () => {
    setIsLoading(true);
    await axios
      .get(`${BASE_URL}Users/GetUsers`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
        },
      })
      .then((response) => {
        setUsers(response.data);
        setIsLoading(false);
      })
      .catch(() => {
        setIsLoading(false);
        alert(
          "Duomenų nuskaitymas nepavyko, bandykite dar kartą. Jei vistiek nepavyksta, kreipkitės į administratorių"
        );
      });
  };

  const getValidationErrors = () => {
    const errors = [];
    if (!name) {
      errors.push("\nVardas yra privalomas laukas.");
    }
    if (!email) {
      errors.push("\nElektroninis paštas yra privalomas laukas.");
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      errors.push("\nElektroninis paštas nurodytas neteisingai.");
    }

    return errors;
  };

  const handleAddUserClick = async () => {
    let validationErrors = getValidationErrors();
    if (validationErrors.length > 0) {
      alert("Užpildyti laukai turi klaidų:\n\n" + validationErrors.join(""));
      return;
    }
    setIsLoading(true);
    await axios
      .put(
        `${BASE_URL}Users/AddUser`,
        {
          name: name,
          email: email,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
          },
        }
      )
      .catch(() => {
        setIsLoading(false);
        alert(
          "Saugojimas nepavyko, bandykite dar kartą. Jei vistiek nepavyksta, kreipkitės į administratorių"
        );
      });

    setShouldFetch(true);
    setName("");
    setEmail("");
    setIsLoading(false);
  };

  const handleRemoveUserClick = async (userId: string) => {
    setIsLoading(true);
    await axios
      .delete(`${BASE_URL}Users/RemoveUser/${userId}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
        },
      })
      .catch(() => {
        setIsLoading(false);
        alert(
          "Saugojimas nepavyko, bandykite dar kartą. Jei vistiek nepavyksta, kreipkitės į administratorių"
        );
      });

    setShouldFetch(true);
    setIsLoading(false);
  };

  const handleGrantAdminRoleClick = async (userId: string) => {
    setIsLoading(true);
    await axios
      .patch(
        `${BASE_URL}Users/GrantAdminRole`,
        {
          userId: userId,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
          },
        }
      )
      .catch(() => {
        setIsLoading(false);
        alert(
          "Saugojimas nepavyko, bandykite dar kartą. Jei vistiek nepavyksta, kreipkitės į administratorių"
        );
      });

    setShouldFetch(true);
    setIsLoading(false);
  };

  const BASE_URL = import.meta.env.VITE_BASE_URL;

  const handleRevokeAdminRoleClick = async (userId: string) => {
    setIsLoading(true);
    await axios
      .patch(
        `${BASE_URL}Users/RevokeAdminRole`,
        {
          userId: userId,
        },
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
          },
        }
      )
      .catch(() => {
        setIsLoading(false);
        alert(
          "Saugojimas nepavyko, bandykite dar kartą. Jei vistiek nepavyksta, kreipkitės į administratorių"
        );
      });

    setShouldFetch(true);
    setIsLoading(false);
  };

  const isAdmin = (userRoles: string[]): boolean => {
    return userRoles.includes("Admin");
  };

  return (
    <MainLayout>
      {isLoading && <LoadinBackdrop />}
      <div className="overflow-x-auto">
        <Table hoverable>
          <Table.Head>
            <Table.HeadCell>Nuotrauka</Table.HeadCell>
            <Table.HeadCell>Vardas</Table.HeadCell>
            <Table.HeadCell>El. paštas</Table.HeadCell>
            <Table.HeadCell>Rolės</Table.HeadCell>
            <Table.HeadCell>
              <span className="sr-only">Edit</span>
            </Table.HeadCell>
          </Table.Head>
          <Table.Body className="divide-y">
            {users.map((user) => (
              <Table.Row className="bg-white dark:border-gray-700 dark:bg-gray-800">
                <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                  {user.profilePhoto.length != 0 && (
                    <img
                      className="w-8 h-8 rounded-full"
                      src={user.profilePhoto}
                      alt="user photo"
                    ></img>
                  )}
                </Table.Cell>
                <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                  {user.name}
                </Table.Cell>
                <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                  {user.email}
                </Table.Cell>
                <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                  {user.userRoleNames.join(", ")}
                </Table.Cell>
                <Table.Cell>
                  {currentUser?.id != user.id && (
                    <ul>
                      <a
                        onClick={() => handleRemoveUserClick(user.id)}
                        className="block cursor-pointer font-medium text-red-600 hover:underline dark:text-red-500"
                      >
                        Naikinti
                      </a>
                      <a
                        onClick={() =>
                          isAdmin(user.userRoleNames)
                            ? handleRevokeAdminRoleClick(user.id)
                            : handleGrantAdminRoleClick(user.id)
                        }
                        className="block cursor-pointer font-medium text-blue-600 hover:underline dark:text-blue-500"
                      >
                        {isAdmin(user.userRoleNames)
                          ? "Atimti administratoriaus teises"
                          : "Suteikti administratoriaus teises"}
                      </a>
                    </ul>
                  )}
                </Table.Cell>
              </Table.Row>
            ))}
            <Table.Row className="bg-white dark:border-gray-700 dark:bg-gray-800">
              <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white"></Table.Cell>
              <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                <input
                  type="text"
                  key="user-name"
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Vardas"
                  value={name}
                  className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                ></input>
              </Table.Cell>
              <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white">
                <input
                  type="text"
                  key="user-email"
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="Elektroninis paštas"
                  value={email}
                  className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                ></input>
              </Table.Cell>
              <Table.Cell className="whitespace-nowrap font-medium text-gray-900 dark:text-white"></Table.Cell>
              <Table.Cell>
                <a
                  onClick={handleAddUserClick}
                  className="font-medium cursor-pointer text-blue-600 hover:underline dark:text-blue-500"
                >
                  Pridėti
                </a>
              </Table.Cell>
            </Table.Row>
          </Table.Body>
        </Table>
      </div>
    </MainLayout>
  );
}

export default AdminToolkitPage;
