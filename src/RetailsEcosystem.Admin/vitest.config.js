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
      "@utils": resolve(__dirname, "src/utils"),
      "@hooks": resolve(__dirname, "src/hooks"),
      "@components": resolve(__dirname, "src/components"),
      "@contexts": resolve(__dirname, "src/contexts"),
      "@configs": resolve(__dirname, "src/configs"),
      "@pages": resolve(__dirname, "src/pages"),
      "@routes": resolve(__dirname, "src/routes"),
      "@layouts": resolve(__dirname, "src/layouts"),
    },
  },

  test: {
    environment: "jsdom",
    globals: true,
    setupFiles: ["./tests/setup.ts"],

    coverage: {
      provider: "v8",

      reporter: ["text", "html", "lcov"],

      include: ["src/**/*.{js,jsx,ts,tsx}"],

      exclude: [
        "src/main.tsx",
        "src/configs/**",
        "src/**/*.d.ts",
      ],

      all: true,

      reportsDirectory: "./coverage",
    },
  },
});
