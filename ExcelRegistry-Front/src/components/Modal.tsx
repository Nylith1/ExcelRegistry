import { FormEvent, ReactNode } from "react";

interface ModalProps {
  name: string;
  children: ReactNode;
  isOpen: boolean;
  onClose: () => void;
}

function Modal({ name, children, isOpen, onClose }: ModalProps) {
  const onModalSubmit = (event: FormEvent<HTMLDivElement>) => {
    event.preventDefault();
  };

  return (
    <div
      className={`${isOpen ? "" : "hidden"}`}
      onSubmit={(e) => onModalSubmit(e)}
    >
      <div className="fixed pt-4 inset-0 flex items-top justify-center bg-gray-800 bg-opacity-50 z-50">
        <div className="relative w-full max-w-md max-h-full">
          <div className="bg-white p-6 rounded shadow-md">
            <button
              type="button"
              className="absolute top-3 right-2.5 text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm w-8 h-8 ml-auto inline-flex justify-center items-center dark:hover:bg-gray-600 dark:hover:text-white"
              onClick={onClose}
            >
              <svg
                className="w-3 h-3"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 14 14"
              >
                <path
                  stroke="currentColor"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="m1 1 6 6m0 0 6 6M7 7l6-6M7 7l-6 6"
                />
              </svg>
              <span className="sr-only">Close modal</span>
            </button>
            <h3 className="mb-4 text-xl font-medium text-gray-900 dark:text-white">
              {name}
            </h3>
            {children}
          </div>
        </div>
      </div>
    </div>
  );
}

export default Modal;
