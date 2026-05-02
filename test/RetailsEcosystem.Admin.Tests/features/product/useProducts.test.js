import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";

const mockGetProducts = vi.fn();

vi.mock("@features/product/productApi", () => ({
  getProducts: mockGetProducts,
}));

const { useProducts } = await import("@features/product/useProducts");

describe("useProducts", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("fetches products on mount with correct params", async () => {
    mockGetProducts.mockResolvedValue({
      items: [{ id: 1, name: "Phone" }],
      totalPage: 1,
    });

    const { result } = renderHook(() => useProducts(1, null, null));

    await waitFor(() => {
      expect(result.current.products).toHaveLength(1);
    });

    expect(mockGetProducts).toHaveBeenCalledWith({
      pageNumber: 1,
      categoryId: null,
      search: null,
    });
  });

  it("returns empty items and zero totalPage on API error", async () => {
    mockGetProducts.mockRejectedValue(new Error("Network error"));

    const { result } = renderHook(() => useProducts(1, null, null));

    await waitFor(() => {
      expect(mockGetProducts).toHaveBeenCalledTimes(1);
    });

    expect(result.current.products).toEqual([]);
    expect(result.current.totalPage).toBe(0);
  });

  it("passes categoryId to API when filter is set", async () => {
    mockGetProducts.mockResolvedValue({ items: [], totalPage: 0 });

    renderHook(() => useProducts(1, 5, null));

    await waitFor(() => {
      expect(mockGetProducts).toHaveBeenCalledWith(
        expect.objectContaining({ categoryId: 5 })
      );
    });
  });

  it("passes search term to API when provided", async () => {
    mockGetProducts.mockResolvedValue({ items: [], totalPage: 0 });

    renderHook(() => useProducts(1, null, "laptop"));

    await waitFor(() => {
      expect(mockGetProducts).toHaveBeenCalledWith(
        expect.objectContaining({ search: "laptop" })
      );
    });
  });

  it("refetches when pageNumber changes", async () => {
    mockGetProducts.mockResolvedValue({ items: [], totalPage: 2 });

    const { rerender } = renderHook(
      ({ page }) => useProducts(page, null, null),
      { initialProps: { page: 1 } }
    );

    await waitFor(() => expect(mockGetProducts).toHaveBeenCalledTimes(1));

    rerender({ page: 2 });

    await waitFor(() => expect(mockGetProducts).toHaveBeenCalledTimes(2));

    expect(mockGetProducts).toHaveBeenLastCalledWith(
      expect.objectContaining({ pageNumber: 2 })
    );
  });

  it("exposes fetchProducts to trigger manual refetch", async () => {
    mockGetProducts.mockResolvedValue({ items: [], totalPage: 0 });

    const { result } = renderHook(() => useProducts(1, null, null));

    await waitFor(() => expect(mockGetProducts).toHaveBeenCalledTimes(1));

    await act(async () => {
      await result.current.fetchProducts();
    });

    expect(mockGetProducts).toHaveBeenCalledTimes(2);
  });
});
