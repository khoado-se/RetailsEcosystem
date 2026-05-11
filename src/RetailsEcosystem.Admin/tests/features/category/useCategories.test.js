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

  it("falls back to [] when res.data.items is absent", async () => {
    mockGetCategories.mockResolvedValue({ data: { totalPage: 2 } });

    const { result } = renderHook(() => useCategories());

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.categories).toEqual([]);
    expect(result.current.totalPage).toBe(2);
  });

  it("falls back to totalPage 1 when res.data.totalPage is absent", async () => {
    mockGetCategories.mockResolvedValue({ data: { items: [] } });

    const { result } = renderHook(() => useCategories());

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.totalPage).toBe(1);
  });

  it("uses err.response.data.title as error when present", async () => {
    const err = new Error("ignored");
    err.response = { data: { title: "Validation error" } };
    mockGetCategories.mockRejectedValue(err);

    const { result } = renderHook(() => useCategories());

    await waitFor(() => expect(result.current.error).toBe("Validation error"));
  });

  it("falls back to static message when error has no title or message", async () => {
    mockGetCategories.mockRejectedValue({});

    const { result } = renderHook(() => useCategories());

    await waitFor(() =>
      expect(result.current.error).toBe("Failed to load categories.")
    );
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
