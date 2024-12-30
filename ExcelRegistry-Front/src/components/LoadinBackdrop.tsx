export function LoadinBackdrop() {
  return (
    <div
      className="fixed inset-0 bg-gray-500 bg-opacity-50 flex justify-center items-center z-50"
      style={{
        opacity: 0.5,
        pointerEvents: "all",
      }}
    >
      <div className="text-white text-lg">Loading...</div>
    </div>
  );
}
