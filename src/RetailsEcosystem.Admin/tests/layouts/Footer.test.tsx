import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import Footer from "../../src/layouts/Footer";

describe("Footer", () => {
  it("renders the Footer heading", () => {
    render(<Footer />);
    expect(screen.getByRole("heading", { level: 1 })).toHaveTextContent(
      "This is Footer"
    );
  });
});
