import { defineConfig } from "vitest/config";
import { resolve } from "path";

const testRoot = new URL(".", import.meta.url).pathname.replace(/^\/([A-Z]:)/, "$1");

export default defineConfig({
  resolve: {
    dedupe: ["react", "react-dom"],
    alias: {
      "@features": resolve(testRoot, "../../src/RetailsEcosystem.Admin/src/features"),
      "@services": resolve(testRoot, "../../src/RetailsEcosystem.Admin/src/services"),
      react: resolve(testRoot, "node_modules/react"),
      "react-dom": resolve(testRoot, "node_modules/react-dom"),
      "react/jsx-runtime": resolve(testRoot, "node_modules/react/jsx-runtime"),
    },
  },
  test: {
    environment: "jsdom",
    globals: true,
    setupFiles: ["./setup.js"],
    coverage: {
      provider: "v8",
      reporter: ["text", "lcov"],
    },
  },
});
