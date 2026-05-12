import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";
import { fileURLToPath } from "url";
import { resolve } from "path";

const __dirname = fileURLToPath(new URL(".", import.meta.url));

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@features":   resolve(__dirname, "src/features"),
      "@services":   resolve(__dirname, "src/services"),
      "@utils":      resolve(__dirname, "src/utils"),
      "@hooks":      resolve(__dirname, "src/hooks"),
      "@components": resolve(__dirname, "src/components"),
      "@contexts":   resolve(__dirname, "src/contexts"),
      "@configs":    resolve(__dirname, "src/configs"),
    },
  },
  test: {
    environment: "jsdom",
    globals:     true,
    setupFiles:  ["./tests/setup.ts"],
    coverage: {
      provider:         "v8",
      reporter:         ["text", "html", "lcov"],
      include:          ["src/**/*.{js,jsx,ts,tsx}"],
      exclude: [
        "src/**/*Api.ts",
        "src/features/auth/**",
        "src/main.tsx",
        "src/configs/**",
      ],
      all:              false,
      reportsDirectory: "./coverage",
    },
  },
});
