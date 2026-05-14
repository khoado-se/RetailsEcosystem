import { describe, it, expect, vi, beforeEach } from "vitest";

const mockGet = vi.fn();
const mockPut = vi.fn();

vi.mock("@services/apiClient", () => ({
  default: { get: mockGet, put: mockPut },
}));

const { getOrders, getOrderById, updateOrderStatus, getOrderStats } =
  await import("@features/order/orderApi");

describe("orderApi", () => {
  beforeEach(() => vi.clearAllMocks());

  it("getOrders omits status param when status is null", async () => {
    mockGet.mockResolvedValue({ data: {} });
    await getOrders({ pageNumber: 1, pageSize: 10, status: null });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.status).toBeUndefined();
  });

  it("getOrders includes status param when not null", async () => {
    mockGet.mockResolvedValue({ data: {} });
    await getOrders({ pageNumber: 1, pageSize: 10, status: 2 });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.status).toBe(2);
  });

  it("getOrders includes status=0 (falsy but not null)", async () => {
    mockGet.mockResolvedValue({ data: {} });
    await getOrders({ pageNumber: 1, status: 0 });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.status).toBe(0);
  });

  it("getOrderById GETs /orders/:id", async () => {
    mockGet.mockResolvedValue({ data: { id: 7 } });
    await getOrderById(7);
    expect(mockGet).toHaveBeenCalledWith("/orders/7");
  });

  it("updateOrderStatus PUTs to /orders/:id/status", async () => {
    mockPut.mockResolvedValue({ data: null });
    await updateOrderStatus(7, 2);
    expect(mockPut).toHaveBeenCalledWith("/orders/7/status", { status: 2 });
  });

  it("getOrderStats GETs /orders/stats", async () => {
    mockGet.mockResolvedValue({ data: { totalOrders: 10 } });
    await getOrderStats();
    expect(mockGet).toHaveBeenCalledWith("/orders/stats");
  });
});
