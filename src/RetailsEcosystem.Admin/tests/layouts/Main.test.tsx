import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import Main from "../../src/layouts/Main";

describe("Main", () => {
  it("renders children inside a <main> element", () => {
    render(<Main><span>child content</span></Main>);
    const main = screen.getByRole("main");
    expect(main).toBeInTheDocument();
    expect(main).toHaveTextContent("child content");
  });

  it("applies the correct Bootstrap classes", () => {
    render(<Main><div /></Main>);
    const main = screen.getByRole("main");
    expect(main.className).toContain("col-md-9");
    expect(main.className).toContain("ms-md-auto");
    expect(main.className).toContain("col-lg-10");
    expect(main.className).toContain("px-md-4");
  });
});
