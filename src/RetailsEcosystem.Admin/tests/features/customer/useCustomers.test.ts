import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";

const mockGetCustomers = vi.fn();

vi.mock("@features/customer/customerApi", () => ({
  getCustomers: mockGetCustomers,
}));

const { useCustomers } = await import("@features/customer/useCustomers");

describe("useCustomers", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("fetches customers on mount and exposes them", async () => {
    mockGetCustomers.mockResolvedValue({
      items: [{ id: "u1", email: "alice@example.com" }],
      totalPage: 1,
    });

    const { result } = renderHook(() => useCustomers(1, null));

    await waitFor(() => {
      expect(result.current.customers).toHaveLength(1);
    });

    expect(mockGetCustomers).toHaveBeenCalledTimes(1);
  });

  it("starts with empty customers before the fetch resolves", () => {
    mockGetCustomers.mockReturnValue(new Promise(() => {}));

    const { result } = renderHook(() => useCustomers(1, null));

    expect(result.current.customers).toEqual([]);
  });

  it("keeps empty customers on API error without throwing", async () => {
    mockGetCustomers.mockRejectedValue(new Error("Network error"));

    const { result } = renderHook(() => useCustomers(1, null));

    await waitFor(() => expect(mockGetCustomers).toHaveBeenCalledTimes(1));

    expect(result.current.customers).toEqual([]);
  });

  it("passes pageNumber and search params to the API", async () => {
    mockGetCustomers.mockResolvedValue({ items: [], totalPage: 0 });

    renderHook(() => useCustomers(2, "Bob"));

    await waitFor(() => {
      expect(mockGetCustomers).toHaveBeenCalledWith(
        expect.objectContaining({ pageNumber: 2, search: "Bob" })
      );
    });
  });

  it("falls back to empty items and 0 totalPage when fields are absent", async () => {
    mockGetCustomers.mockResolvedValue({});

    const { result } = renderHook(() => useCustomers(1, null));

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.customers).toEqual([]);
    expect(result.current.totalPage).toBe(0);
  });

  it("fetchCustomers triggers a manual refetch", async () => {
    mockGetCustomers.mockResolvedValue({ items: [], totalPage: 0 });

    const { result } = renderHook(() => useCustomers(1, null));

    await waitFor(() => expect(mockGetCustomers).toHaveBeenCalledTimes(1));

    await act(async () => {
      await result.current.fetchCustomers();
    });

    expect(mockGetCustomers).toHaveBeenCalledTimes(2);
  });
});
