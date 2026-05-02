document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll(".password-toggle").forEach(btn => {
    btn.addEventListener("click", () => {
      const input = btn.closest(".input-group").querySelector("input");
      const icon = btn.querySelector("i");
      if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("opacity-50");
      } else {
        input.type = "password";
        icon.classList.add("opacity-50");
      }
    });
  });
});
