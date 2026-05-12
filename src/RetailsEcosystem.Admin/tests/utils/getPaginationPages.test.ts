import { describe, it, expect } from "vitest";
import {
  getPaginationPages,
  shouldShowLeftDots,
  shouldShowRightDots,
} from "@utils/getPaginationPages";

describe("getPaginationPages", () => {
  it("returns empty array when totalPages is zero", () => {
    expect(getPaginationPages(1, 0)).toEqual([]);
  });

  it("returns empty array when totalPages is negative", () => {
    expect(getPaginationPages(1, -1)).toEqual([]);
  });

  it("returns [1] for a single-page result", () => {
    expect(getPaginationPages(1, 1)).toEqual([1]);
  });

  it("returns first 5 pages when near the beginning", () => {
    expect(getPaginationPages(1, 10)).toEqual([1, 2, 3, 4, 5]);
  });

  it("returns centred window when in the middle", () => {
    expect(getPaginationPages(5, 10)).toEqual([3, 4, 5, 6, 7]);
  });

  it("returns last 5 pages when near the end", () => {
    expect(getPaginationPages(10, 10)).toEqual([6, 7, 8, 9, 10]);
  });

  it("respects custom maxVisiblePages", () => {
    expect(getPaginationPages(2, 5, 3)).toEqual([1, 2, 3]);
  });

  it("does not exceed totalPages in the window", () => {
    const pages = getPaginationPages(1, 3);
    expect(pages.every((p) => p <= 3)).toBe(true);
  });

  it("does not include page numbers below 1", () => {
    const pages = getPaginationPages(1, 10);
    expect(pages.every((p) => p >= 1)).toBe(true);
  });
});

describe("shouldShowLeftDots", () => {
  it("returns false when current page is at the start", () => {
    expect(shouldShowLeftDots(1, 5)).toBe(false);
  });

  it("returns false when current page is within nearStartLimit", () => {
    expect(shouldShowLeftDots(3, 5)).toBe(false);
  });

  it("returns true when current page is beyond nearStartLimit", () => {
    expect(shouldShowLeftDots(4, 5)).toBe(true);
  });
});

describe("shouldShowRightDots", () => {
  it("returns false when current page is at the last page", () => {
    expect(shouldShowRightDots(10, 10, 5)).toBe(false);
  });

  it("returns false when current page is near the end", () => {
    expect(shouldShowRightDots(9, 10, 5)).toBe(false);
  });

  it("returns true when current page is far from the end", () => {
    expect(shouldShowRightDots(1, 10, 5)).toBe(true);
  });
});
