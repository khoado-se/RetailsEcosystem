import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";
import { fileURLToPath } from "url";
import { resolve } from "path";

const __dirname = fileURLToPath(new URL(".", import.meta.url));

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@features": resolve(__dirname, "src/features"),
      "@services": resolve(__dirname, "src/services"),
      "@utils":    resolve(__dirname, "src/utils"),
    },
  },
  test: {
    environment: "jsdom",
    globals:     true,
    setupFiles:  ["./tests/setup.js"],
    coverage: {
      provider:         "v8",
      reporter:         ["text", "html", "lcov"],
      include:          ["src/**/*.{js,jsx,ts,tsx}"],
      exclude: [
        "src/**/*Api.js",
        "src/features/auth/**",
        "src/main.jsx",
        "src/configs/**",
      ],
      all:              false,
      reportsDirectory: "./coverage",
    },
  },
});
