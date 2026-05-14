import { describe, it, expect, vi, beforeEach } from "vitest";

const mockGet = vi.fn();
const mockPut = vi.fn();

vi.mock("@services/apiClient", () => ({
  default: { get: mockGet, put: mockPut },
}));

const { getCustomers, updateCustomerStatus } = await import("@features/customer/customerApi");

describe("customerApi", () => {
  beforeEach(() => vi.clearAllMocks());

  it("getCustomers passes search as undefined when null", async () => {
    mockGet.mockResolvedValue({ data: { items: [] } });
    await getCustomers({ pageNumber: 1, pageSize: 10, search: null });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.search).toBeUndefined();
  });

  it("getCustomers passes search string when provided", async () => {
    mockGet.mockResolvedValue({ data: { items: [] } });
    await getCustomers({ pageNumber: 1, pageSize: 10, search: "alice" });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.search).toBe("alice");
  });

  it("getCustomers returns res.data", async () => {
    mockGet.mockResolvedValue({ data: { items: [{ id: "u1" }], totalPage: 1 } });
    const result = await getCustomers({ pageNumber: 1, pageSize: 10, search: null });
    expect(result).toEqual({ items: [{ id: "u1" }], totalPage: 1 });
  });

  it("updateCustomerStatus PUTs to /customers/:id/status", async () => {
    mockPut.mockResolvedValue({ data: null });
    await updateCustomerStatus("u1", true);
    expect(mockPut).toHaveBeenCalledWith("/customers/u1/status", { isActive: true });
  });

  it("updateCustomerStatus with isActive=false", async () => {
    mockPut.mockResolvedValue({ data: null });
    await updateCustomerStatus("u2", false);
    expect(mockPut).toHaveBeenCalledWith("/customers/u2/status", { isActive: false });
  });
});
