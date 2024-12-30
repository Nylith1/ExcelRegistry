/** @type {import('tailwindcss').Config} */
export default {
  darkMode: "class",
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {},
  },
  plugins: [require("flowbite/plugin")],
};
// module.exports = {
//   content: [
//     './src/**/*.{js,jsx,ts,tsx}', // Adjust this based on your project structure
//     './node_modules/flowbite-react/**/*.{js,jsx,ts,tsx}', // Include Flowbite paths
//   ],
//   theme: {
//     extend: {},
//   },
//   plugins: [
//     require('flowbite/plugin'), // Add Flowbite plugin
//   ],
// };
