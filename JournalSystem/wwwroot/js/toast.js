window.showToast = (message, type) => {
  Toastify({
    text: message,
    duration: 3000,
    gravity: "bottom",
    position: "right",
    backgroundColor:
      type === "success" ? "#16a34a" : type === "error" ? "#dc2626" : "#334155",
  }).showToast();
};
