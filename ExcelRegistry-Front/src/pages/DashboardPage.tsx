import { useEffect, useState } from "react";
import MainLayout from "../components/MainLayout";
import axios from "axios";
import CountdownTimer from "../components/CountdownTimer";
import { format } from "date-fns";
import "flowbite/dist/flowbite.min.css";
import { Datepicker } from "flowbite-react";
import { RegentDto, UserDto } from "../assets/Models";
import { LoadinBackdrop } from "../components/LoadinBackdrop";
import { TimeZoneOffset } from "../Helpers/TimeZoneOffset";

function DashboardPage() {
  const [shouldFetch, setShouldFetch] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [regentsData, setRegentsData] = useState<RegentDto[]>([]);

  const [editingRowId, setEditingRowId] = useState(null);

  const [solutionBeingPrepared, setSolutionBeingPrepared] = useState("");
  const [dateOfManufacture, setDateOfManufacture] = useState<Date>(new Date());

  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  const [expireDate, setExpireDate] = useState<Date>(tomorrow);
  const [user, setUser] = useState<UserDto | null>(null);

  const [editingSolutionBeingPrepared, setEditingSolutionBeingPrepared] =
    useState("");
  const [editingDateOfManufacture, setEditingDateOfManufacture] =
    useState<Date | null>(null);
  const [editingExpireDate, setEditingExpireDate] = useState<Date | null>(null);

  const [placeholderIndex, setPlaceholderIndex] = useState("");
  const [placeholderDate, setPlaceholderDate] = useState<Date>(new Date());
  const [palceholderUserName, setPalceholderUserName] = useState("");

  const BASE_URL = import.meta.env.VITE_BASE_URL;

  useEffect(() => {
    const loadUserAndFetchRegents = async () => {
      const savedUser = localStorage.getItem("user");
      if (savedUser) {
        setUser(JSON.parse(savedUser));
      }

      if (shouldFetch) {
        setIsLoading(true);
        await fetchRegents();
        setIsLoading(false);
      }
    };

    setPlaceholderIndex(
      (
        parseInt(regentsData[regentsData.length - 1]?.indexNumber) + 1
      ).toString()
    );
    setPlaceholderDate(new Date());
    setPalceholderUserName(user?.name || "");
    loadUserAndFetchRegents();
  }, [shouldFetch]);

  useEffect(() => {
    const intervalId = setInterval(() => {
      setPlaceholderDate(new Date());
    }, 1000);

    return () => clearInterval(intervalId);
  }, []);

  const handleEditClick = (id: any) => {
    setEditingRowId(id);

    const regent = regentsData.find((x) => x.id === id);
    if (regent != undefined) {
      setEditingSolutionBeingPrepared(regent.solutionBeingPrepared);
      setEditingDateOfManufacture(new Date(regent.dateOfManufacture));
      setEditingExpireDate(new Date(regent.expireDate));
    }
  };

  const handleCancelClick = () => {
    setEditingRowId(null);
    setEditingSolutionBeingPrepared("");
    setEditingDateOfManufacture(null);
    setEditingExpireDate(null);
  };

  const getValidationErrors = () => {
    const errors = [];
    if (!solutionBeingPrepared) {
      errors.push("\nRuošiamas tirpalas yra privalomas laukas.");
    }
    if (!dateOfManufacture) {
      errors.push("\nPagaminimo data yra privalomas laukas.");
    }
    if (!expireDate) {
      errors.push("\nGaliojimo data yra privalomas laukas.");
    }

    return errors;
  };

  const handleSaveClick = async () => {
    let validationErrors = getValidationErrors();
    if (validationErrors.length > 0) {
      alert("Užpildyti laukai turi klaidų:\n\n" + validationErrors.join(""));
      return;
    }
    setIsLoading(true);
    const id = crypto.randomUUID().toString();
    await axios
      .post(
        `${BASE_URL}RegentsRegister/AddRegent`,
        {
          id: id,
          date: new Date().toISOString(),
          solutionBeingPrepared: solutionBeingPrepared,
          dateOfManufacture: dateOfManufacture.toISOString(),
          expireDate: expireDate.toISOString(),
          user: user?.name,
          timeZoneOffsetInHours: TimeZoneOffset.getTimezoneOffsetInHours(),
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
    resetInputState();
    setIsLoading(false);
  };

  const fetchRegents = async () => {
    setIsLoading(true);
    axios
      .get(`${BASE_URL}RegentsRegister/GetRegents`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwtToken")}`,
        },
      })
      .then((response) => {
        setRegentsData(response.data);
      })
      .finally(() => {
        setShouldFetch(false);
        setIsLoading(false);
      })
      .catch(() => {
        setIsLoading(false);
        alert(
          "Duomenų nuskaitymas nepavyko, bandykite dar kartą. Jei vistiek nepavyksta, kreipkitės į administratorių"
        );
      });
  };

  const resetInputState = () => {
    setSolutionBeingPrepared("");
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    setDateOfManufacture(new Date());
    setExpireDate(tomorrow);
  };

  function getCountdownSeconds(regentRegisterDate: Date): number {
    let nowInMiliseconds = new Date().getTime();
    let regentRegisterDateInMiliseconds = new Date(
      regentRegisterDate
    ).getTime();
    let possibleToEditTime = regentRegisterDateInMiliseconds + 15 * 60 * 1000;

    let differenceInMilliseconds = possibleToEditTime - nowInMiliseconds;
    let differenceInSeconds = Math.floor(differenceInMilliseconds / 1000);

    return differenceInSeconds;
  }

  const handleEditSaveClick = async () => {
    setIsLoading(true);
    const regent = regentsData.find((x) => x.id === editingRowId);
    console.log(regent);
    if (regent != undefined) {
      await axios
        .patch(
          `${BASE_URL}RegentsRegister/EditRegent`,
          {
            id: editingRowId,
            indexNumber: regent.indexNumber,
            date: new Date(regent.date).toISOString(),
            solutionBeingPrepared: editingSolutionBeingPrepared,
            dateOfManufacture: new Date(
              editingDateOfManufacture!
            ).toISOString(),
            expireDate: new Date(editingExpireDate!).toISOString(),
            user: user?.name,
            timeZoneOffsetInHours: TimeZoneOffset.getTimezoneOffsetInHours(),
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

      regent.solutionBeingPrepared = editingSolutionBeingPrepared;
      regent.dateOfManufacture = editingDateOfManufacture || new Date();
      regent.expireDate = editingExpireDate || new Date();

      setEditingRowId(null);
      setEditingSolutionBeingPrepared("");
      setEditingDateOfManufacture(null);
      setEditingExpireDate(null);
    }
    setIsLoading(false);
  };

  return (
    <MainLayout>
      {isLoading && <LoadinBackdrop />}

      <div className="overflow-auto h-full">
        <table className="w-full text-sm text-left rtl:text-right text-gray-500 dark:text-gray-400">
          <thead className="text-xs text-gray-700 uppercase bg-gray-50 dark:bg-gray-700 dark:text-gray-400">
            <tr>
              <th scope="col" className="px-6 py-3">
                Eilės numeris
              </th>
              <th scope="col" className="px-6 py-3">
                Data
              </th>
              <th scope="col" className="px-6 py-3">
                Ruošiamas tirpalas
              </th>
              <th scope="col" className="px-6 py-3">
                Pagaminimo data
              </th>
              <th scope="col" className="px-6 py-3">
                Galiojimo data
              </th>
              <th scope="col" className="px-6 py-3">
                Įrašą atlikęs asmuo
              </th>
              <th scope="col" className="px-6 py-3"></th>
            </tr>
          </thead>
          <tbody>
            {regentsData.map((regent) =>
              editingRowId === regent.id ? (
                <tr key={regent.id} className="bg-white dark:bg-gray-800">
                  <td className="px-6 py-4">{regent.indexNumber}</td>
                  <td className="px-6 py-4">
                    {format(new Date(regent.date), "yyyy-MM-dd HH:mm:ss")}
                  </td>
                  <td className="px-6 py-4">
                    <input
                      id="solution-being-prepared-edit"
                      type="text"
                      value={editingSolutionBeingPrepared}
                      onChange={(e) =>
                        setEditingSolutionBeingPrepared(e.target.value)
                      }
                      className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg"
                    />
                  </td>
                  <td className="px-6 py-4">
                    <Datepicker
                      onChange={(date) => setEditingDateOfManufacture(date)}
                      value={editingDateOfManufacture}
                      language="lt-LT"
                      labelTodayButton="Šiandiena"
                      labelClearButton="Išvalyti"
                      id="date-of-manufacture-edit"
                    />
                  </td>
                  <td className="px-6 py-4">
                    <Datepicker
                      onChange={(date) => setEditingExpireDate(date)}
                      value={editingExpireDate}
                      language="lt-LT"
                      labelTodayButton="Šiandiena"
                      labelClearButton="Išvalyti"
                      id="expire-date-edit"
                    />
                  </td>
                  <td className="px-6 py-4">{regent.user}</td>
                  <td className="px-6 py-4">
                    <div className="flex flex-col items-center w-40 space-y-2">
                      <button
                        onClick={handleEditSaveClick}
                        className="font-medium text-blue-600 dark:text-blue-500 hover:underline"
                      >
                        Saugoti
                      </button>
                      <button
                        onClick={handleCancelClick}
                        className="font-medium text-red-600 dark:text-blue-500 hover:underline"
                      >
                        Atšaukti
                      </button>
                    </div>
                  </td>
                </tr>
              ) : (
                <tr
                  key={regent.id}
                  className="bg-white border-b dark:bg-gray-800 dark:border-gray-700"
                >
                  <td className="px-6 py-4">{regent.indexNumber}</td>
                  <td className="px-6 py-4">
                    {format(new Date(regent.date), "yyyy-MM-dd HH:mm:ss")}
                  </td>
                  <td className="px-6 py-4">{regent.solutionBeingPrepared}</td>
                  <td className="px-6 py-4">
                    {format(new Date(regent.dateOfManufacture), "yyyy-MM-dd")}
                  </td>
                  <td className="px-6 py-4">
                    {format(new Date(regent.expireDate), "yyyy-MM-dd")}
                  </td>
                  <td className="px-6 py-4">{regent.user}</td>
                  <td className="px-6 py-4">
                    <button
                      onClick={() => handleEditClick(regent.id)}
                      className="font-medium text-blue-600 dark:text-blue-500 hover:underline"
                    >
                      <CountdownTimer
                        text="Koreguoti:"
                        countdownSeconds={getCountdownSeconds(regent.date)}
                      ></CountdownTimer>
                    </button>
                  </td>
                </tr>
              )
            )}
            <tr className="bg-white dark:bg-gray-800 h-[50vh] align-top max-h-96 overflow-y-auto">
              <td className="px-6 py-4">{placeholderIndex}</td>
              <td className="px-6 py-4">
                {format(new Date(placeholderDate), "yyyy-MM-dd HH:mm:ss")}
              </td>
              <td className="px-6 py-4">
                <input
                  type="text"
                  id="solution-being-prepared"
                  onChange={(e) => setSolutionBeingPrepared(e.target.value)}
                  placeholder="Ruošiamas tirpalas"
                  value={solutionBeingPrepared}
                  className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                ></input>
              </td>
              <td className="px-6 py-4">
                <Datepicker
                  onChange={(date) => setDateOfManufacture(date || new Date())}
                  value={dateOfManufacture}
                  language="lt-LT"
                  labelTodayButton="Šiandiena"
                  labelClearButton="Išvalyti"
                  id="date-of-manufacture"
                />
              </td>
              <td className="px-6 py-4">
                <Datepicker
                  onChange={(date) => setExpireDate(date || new Date())}
                  value={expireDate}
                  language="lt-LT"
                  labelTodayButton="Šiandiena"
                  labelClearButton="Išvalyti"
                  id="expire-date"
                />
              </td>
              <td className="px-6 py-4">{palceholderUserName}</td>
              <td className="px-6 py-4 items">
                <div className="flex flex-col items-center w-40 space-y-2">
                  <button
                    onClick={() => handleSaveClick()}
                    className="font-medium text-blue-600 dark:text-blue-500 hover:underline"
                  >
                    Saugoti
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </MainLayout>
  );
}

export default DashboardPage;
