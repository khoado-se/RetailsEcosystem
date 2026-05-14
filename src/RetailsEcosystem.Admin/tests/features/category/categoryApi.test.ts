import { describe, it, expect, vi, beforeEach } from "vitest";

const mockGet = vi.fn();
const mockPost = vi.fn();
const mockPut = vi.fn();
const mockDelete = vi.fn();

vi.mock("@services/apiClient", () => ({
  default: { get: mockGet, post: mockPost, put: mockPut, delete: mockDelete },
}));

const { getCategories, getCategoryById, createCategory, updateCategory, deleteCategory } =
  await import("@features/category/categoryApi");

describe("categoryApi", () => {
  beforeEach(() => vi.clearAllMocks());

  it("getCategories GETs /categories with pagination params", async () => {
    mockGet.mockResolvedValue({ data: { items: [] } });
    await getCategories({ pageNumber: 2, pageSize: 5 });
    expect(mockGet).toHaveBeenCalledWith("/categories", { params: { pageNumber: 2, pageSize: 5 } });
  });

  it("getCategories uses defaults when no params", async () => {
    mockGet.mockResolvedValue({ data: {} });
    await getCategories();
    expect(mockGet).toHaveBeenCalledWith("/categories", { params: { pageNumber: 1, pageSize: 10 } });
  });

  it("getCategoryById GETs /categories/:id", async () => {
    mockGet.mockResolvedValue({ data: { id: 3 } });
    await getCategoryById(3);
    expect(mockGet).toHaveBeenCalledWith("/categories/3");
  });

  it("createCategory POSTs to /categories", async () => {
    mockPost.mockResolvedValue({ data: { id: 10 } });
    await createCategory({ name: "Electronics", description: "Tech" });
    expect(mockPost).toHaveBeenCalledWith("/categories", { name: "Electronics", description: "Tech" });
  });

  it("updateCategory PUTs to /categories/:id", async () => {
    mockPut.mockResolvedValue({ data: {} });
    await updateCategory(3, { id: 3, name: "Updated", description: null });
    expect(mockPut).toHaveBeenCalledWith("/categories/3", { id: 3, name: "Updated", description: null });
  });

  it("deleteCategory DELETEs /categories/:id", async () => {
    mockDelete.mockResolvedValue({ data: null });
    await deleteCategory(3);
    expect(mockDelete).toHaveBeenCalledWith("/categories/3");
  });
});
