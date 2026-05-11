import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";

const mockGetOrders = vi.fn();

vi.mock("@features/order/orderApi", () => ({
  getOrders: mockGetOrders,
}));

// useOrders reads res.data.items and res.data.totalPage
const { useOrders } = await import("@features/order/useOrders");

describe("useOrders", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("fetches orders on mount and exposes them", async () => {
    mockGetOrders.mockResolvedValue({
      data: { items: [{ id: 1, status: "Pending" }], totalPage: 1 },
    });

    const { result } = renderHook(() => useOrders(1, null));

    await waitFor(() => {
      expect(result.current.orders).toHaveLength(1);
    });

    expect(mockGetOrders).toHaveBeenCalledTimes(1);
  });

  it("sets error state when the API rejects", async () => {
    mockGetOrders.mockRejectedValue(new Error("Server error"));

    const { result } = renderHook(() => useOrders(1, null));

    await waitFor(() => {
      expect(result.current.error).toBe("Server error");
    });

    expect(result.current.orders).toEqual([]);
  });

  it("passes statusFilter to the API call", async () => {
    mockGetOrders.mockResolvedValue({ data: { items: [], totalPage: 0 } });

    renderHook(() => useOrders(1, "Pending"));

    await waitFor(() => {
      expect(mockGetOrders).toHaveBeenCalledWith(
        expect.objectContaining({ status: "Pending" })
      );
    });
  });

  it("falls back to [] and totalPage 1 when fields are absent in response", async () => {
    mockGetOrders.mockResolvedValue({ data: {} });

    const { result } = renderHook(() => useOrders(1, null));

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.orders).toEqual([]);
    expect(result.current.totalPage).toBe(1);
  });

  it("falls back to static message when error has no message", async () => {
    mockGetOrders.mockRejectedValue({});

    const { result } = renderHook(() => useOrders(1, null));

    await waitFor(() =>
      expect(result.current.error).toBe("Failed to load orders.")
    );
  });

  it("fetchOrders triggers a manual refetch", async () => {
    mockGetOrders.mockResolvedValue({ data: { items: [], totalPage: 0 } });

    const { result } = renderHook(() => useOrders(1, null));

    await waitFor(() => expect(mockGetOrders).toHaveBeenCalledTimes(1));

    await act(async () => {
      await result.current.fetchOrders();
    });

    expect(mockGetOrders).toHaveBeenCalledTimes(2);
  });
});
