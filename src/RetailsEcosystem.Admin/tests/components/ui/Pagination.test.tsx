import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import Pagination from "@components/ui/Pagination";

function renderPagination(pageNumber: number, totalPage: number, setPageNumber = vi.fn()) {
  return { ...render(<Pagination pageNumber={pageNumber} totalPage={totalPage} setPageNumber={setPageNumber} />), setPageNumber };
}

describe("Pagination — prev/next buttons", () => {
  it("prev button is disabled on page 1", () => {
    renderPagination(1, 5);
    const prev = screen.getByText("«").closest("button");
    expect(prev).toBeDisabled();
  });

  it("next button is disabled on last page", () => {
    renderPagination(5, 5);
    const next = screen.getByText("»").closest("button");
    expect(next).toBeDisabled();
  });

  it("clicking prev calls setPageNumber with pageNumber-1", () => {
    const { setPageNumber } = renderPagination(3, 10);
    fireEvent.click(screen.getByText("«"));
    expect(setPageNumber).toHaveBeenCalledWith(2);
  });

  it("clicking next calls setPageNumber with pageNumber+1", () => {
    const { setPageNumber } = renderPagination(3, 10);
    fireEvent.click(screen.getByText("»"));
    expect(setPageNumber).toHaveBeenCalledWith(4);
  });
});

describe("Pagination — page number buttons", () => {
  it("clicking a page number calls setPageNumber", () => {
    const { setPageNumber } = renderPagination(1, 5);
    fireEvent.click(screen.getByText("3"));
    expect(setPageNumber).toHaveBeenCalledWith(3);
  });

  it("active page has active class", () => {
    renderPagination(2, 5);
    const activeBtn = screen.getByText("2").closest("li");
    expect(activeBtn?.className).toContain("active");
  });
});

describe("Pagination — dots", () => {
  it("shows left dots and first-page button when far from start", () => {
    renderPagination(8, 15);
    // left dots: page > 3 → should show "1" button
    const allBtns = screen.getAllByRole("button");
    const labels = allBtns.map((b) => b.textContent);
    expect(labels).toContain("1");
    expect(screen.getAllByText("...").length).toBeGreaterThanOrEqual(1);
  });

  it("shows right dots and last-page button when far from end", () => {
    renderPagination(1, 15);
    const allBtns = screen.getAllByRole("button");
    const labels = allBtns.map((b) => b.textContent);
    expect(labels).toContain("15");
    expect(screen.getAllByText("...").length).toBeGreaterThanOrEqual(1);
  });

  it("clicking first-page button in left dots calls setPageNumber(1)", () => {
    const { setPageNumber } = renderPagination(8, 15);
    // The "1" button in the left dots area
    const btns = screen.getAllByRole("button");
    const btn1 = btns.find((b) => b.textContent === "1");
    if (btn1) fireEvent.click(btn1);
    expect(setPageNumber).toHaveBeenCalledWith(1);
  });

  it("clicking last-page button in right dots calls setPageNumber(totalPage)", () => {
    const { setPageNumber } = renderPagination(1, 15);
    const btns = screen.getAllByRole("button");
    const btn15 = btns.find((b) => b.textContent === "15");
    if (btn15) fireEvent.click(btn15);
    expect(setPageNumber).toHaveBeenCalledWith(15);
  });
});
