import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";

const mockGetCategories = vi.fn();

vi.mock("@features/category/categoryApi", () => ({
  getCategories: mockGetCategories,
}));

const { useCategoriesInfinite } = await import("@features/category/useCategories");

describe("useCategoriesInfinite", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("loadMore fetches page 1 and populates items", async () => {
    mockGetCategories.mockResolvedValue({
      data: { items: [{ id: 1, name: "Cat1" }], totalPage: 3 },
    });

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });

    expect(result.current.items).toEqual([{ id: 1, name: "Cat1" }]);
    expect(result.current.page).toBe(2);
    expect(result.current.hasMore).toBe(true);
    expect(result.current.loading).toBe(false);
  });

  it("loadMore appends items on subsequent pages", async () => {
    mockGetCategories
      .mockResolvedValueOnce({ data: { items: [{ id: 1 }], totalPage: 3 } })
      .mockResolvedValueOnce({ data: { items: [{ id: 2 }], totalPage: 3 } });

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });
    await act(async () => { await result.current.loadMore(); });

    expect(result.current.items).toEqual([{ id: 1 }, { id: 2 }]);
    expect(result.current.page).toBe(3);
  });

  it("loadMore sets hasMore false on last page", async () => {
    mockGetCategories.mockResolvedValue({
      data: { items: [{ id: 1 }], totalPage: 1 },
    });

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });

    expect(result.current.hasMore).toBe(false);
  });

  it("loadMore skips when already loading", async () => {
    let resolveFirst;
    mockGetCategories.mockReturnValue(new Promise((r) => { resolveFirst = r; }));

    const { result } = renderHook(() => useCategoriesInfinite());

    act(() => { result.current.loadMore(); });

    await waitFor(() => expect(result.current.loading).toBe(true));

    act(() => { result.current.loadMore(); });

    expect(mockGetCategories).toHaveBeenCalledTimes(1);

    await act(async () => {
      resolveFirst({ data: { items: [], totalPage: 1 } });
    });
  });

  it("loadMore sets error from err.message on API failure", async () => {
    mockGetCategories.mockRejectedValue(new Error("Network failure"));

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });

    expect(result.current.error).toBe("Network failure");
    expect(result.current.items).toEqual([]);
    expect(result.current.loading).toBe(false);
  });

  it("loadMore sets error from err.response.data.title when present", async () => {
    const err = new Error("ignored");
    err.response = { data: { title: "Bad Request" } };
    mockGetCategories.mockRejectedValue(err);

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });

    expect(result.current.error).toBe("Bad Request");
  });

  it("reset clears items, page, hasMore and error", async () => {
    mockGetCategories.mockResolvedValue({
      data: { items: [{ id: 1 }], totalPage: 3 },
    });

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });

    expect(result.current.items).toHaveLength(1);
    expect(result.current.page).toBe(2);

    act(() => { result.current.reset(); });

    expect(result.current.items).toEqual([]);
    expect(result.current.page).toBe(1);
    expect(result.current.hasMore).toBe(true);
    expect(result.current.error).toBeNull();
  });

  it("loadMore falls back to empty array when data.items is absent", async () => {
    mockGetCategories.mockResolvedValue({
      data: { totalPage: 2 },
    });

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });

    expect(result.current.items).toEqual([]);
  });

  it("loadMore falls back to totalPage 1 when data.totalPage is absent", async () => {
    mockGetCategories.mockResolvedValue({
      data: { items: [{ id: 1 }] },
    });

    const { result } = renderHook(() => useCategoriesInfinite());

    await act(async () => { await result.current.loadMore(); });

    expect(result.current.hasMore).toBe(false);
  });
});
