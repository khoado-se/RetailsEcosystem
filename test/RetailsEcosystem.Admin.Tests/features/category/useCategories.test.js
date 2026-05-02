import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";

const mockGetCategories = vi.fn();

vi.mock("@features/category/categoryApi", () => ({
  getCategories: mockGetCategories,
}));

const { useCategories } = await import("@features/category/useCategories");

describe("useCategories", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("fetches categories on mount", async () => {
    mockGetCategories.mockResolvedValue({
      data: { items: [{ id: 1, name: "Electronics" }] },
    });

    const { result } = renderHook(() => useCategories());

    await waitFor(() => {
      expect(result.current.categories).toHaveLength(1);
    });

    expect(mockGetCategories).toHaveBeenCalledTimes(1);
  });

  it("starts with empty categories before fetch resolves", () => {
    mockGetCategories.mockReturnValue(new Promise(() => {}));

    const { result } = renderHook(() => useCategories());

    expect(result.current.categories).toEqual([]);
  });

  it("does not throw on API error — keeps empty categories", async () => {
    mockGetCategories.mockRejectedValue(new Error("Network error"));

    const { result } = renderHook(() => useCategories());

    await waitFor(() => {
      expect(mockGetCategories).toHaveBeenCalledTimes(1);
    });

    expect(result.current.categories).toEqual([]);
  });

  it("fetchCategories increments tick and triggers a refetch", async () => {
    mockGetCategories.mockResolvedValue({
      data: { items: [{ id: 1, name: "Books" }] },
    });

    const { result } = renderHook(() => useCategories());

    await waitFor(() => expect(mockGetCategories).toHaveBeenCalledTimes(1));

    act(() => {
      result.current.fetchCategories();
    });

    await waitFor(() => expect(mockGetCategories).toHaveBeenCalledTimes(2));
  });
});
