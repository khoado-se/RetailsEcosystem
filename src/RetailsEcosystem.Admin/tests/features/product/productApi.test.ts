import { describe, it, expect, vi, beforeEach } from "vitest";

const mockGet = vi.fn();
const mockPost = vi.fn();
const mockPut = vi.fn();
const mockDelete = vi.fn();

vi.mock("@services/apiClient", () => ({
  default: { get: mockGet, post: mockPost, put: mockPut, delete: mockDelete },
}));

const { getProducts, getProductById, createProduct, updateProduct, deleteProduct } =
  await import("@features/product/productApi");

describe("getProducts", () => {
  beforeEach(() => vi.clearAllMocks());

  it("sends request with required params only when filters are null/undefined", async () => {
    mockGet.mockResolvedValue({ data: { items: [] } });
    await getProducts({ pageNumber: 1, pageSize: 10, categoryId: null, search: null, isFeatured: null, sortBy: null, sortDesc: true });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.categoryId).toBeUndefined();
    expect(params.search).toBeUndefined();
    expect(params.isFeatured).toBeUndefined();
    expect(params.sortBy).toBeUndefined();
    expect(params.sortDesc).toBe(true);
  });

  it("includes all optional params when provided", async () => {
    mockGet.mockResolvedValue({ data: { items: [] } });
    await getProducts({ pageNumber: 2, pageSize: 20, categoryId: 5, search: "phone", isFeatured: true, sortBy: "name", sortDesc: false });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.categoryId).toBe(5);
    expect(params.search).toBe("phone");
    expect(params.isFeatured).toBe(true);
    expect(params.sortBy).toBe("name");
    expect(params.sortDesc).toBe(false);
  });

  it("includes isFeatured=false when explicitly false", async () => {
    mockGet.mockResolvedValue({ data: { items: [] } });
    await getProducts({ pageNumber: 1, pageSize: 10, isFeatured: false });
    const params = mockGet.mock.calls[0][1].params;
    expect(params.isFeatured).toBe(false);
  });

  it("returns res.data", async () => {
    mockGet.mockResolvedValue({ data: { items: [{ id: 1 }], totalPage: 1 } });
    const result = await getProducts({ pageNumber: 1 });
    expect(result).toEqual({ items: [{ id: 1 }], totalPage: 1 });
  });
});

describe("productApi — CRUD", () => {
  beforeEach(() => vi.clearAllMocks());

  it("getProductById GETs /products/:id", async () => {
    mockGet.mockResolvedValue({ data: { id: 5 } });
    await getProductById(5);
    expect(mockGet).toHaveBeenCalledWith("/products/5");
  });

  it("createProduct POSTs to /products", async () => {
    mockPost.mockResolvedValue({ data: { id: 10 } });
    await createProduct({ name: "Phone", price: 999 });
    expect(mockPost).toHaveBeenCalledWith("/products", { name: "Phone", price: 999 });
  });

  it("updateProduct PUTs to /products/:id", async () => {
    mockPut.mockResolvedValue({ data: {} });
    await updateProduct(10, { name: "Phone Pro" });
    expect(mockPut).toHaveBeenCalledWith("/products/10", { name: "Phone Pro" });
  });

  it("deleteProduct DELETEs /products/:id", async () => {
    mockDelete.mockResolvedValue({ data: null });
    await deleteProduct(10);
    expect(mockDelete).toHaveBeenCalledWith("/products/10");
  });
});
