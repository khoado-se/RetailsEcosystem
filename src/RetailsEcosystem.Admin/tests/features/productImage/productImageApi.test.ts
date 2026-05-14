import { describe, it, expect, vi, beforeEach } from "vitest";

const mockPost = vi.fn();
const mockGet = vi.fn();
const mockDelete = vi.fn();

vi.mock("@services/apiClient", () => ({
  default: { post: mockPost, get: mockGet, delete: mockDelete },
}));

const { uploadProductImage, getProductImages, deleteProductImage } =
  await import("@features/productImage/productImageApi");

describe("productImageApi", () => {
  beforeEach(() => vi.clearAllMocks());

  it("uploadProductImage POSTs to correct URL with FormData and overrides Content-Type", async () => {
    mockPost.mockResolvedValue({ data: null });
    const fd = new FormData();
    await uploadProductImage(42, fd);
    expect(mockPost).toHaveBeenCalledWith(
      "/productImages/upload?productId=42",
      fd,
      expect.objectContaining({
        headers: { "Content-Type": undefined },
        timeout: 60000,
      })
    );
  });

  it("getProductImages GETs /productImages/by-product/:id", async () => {
    mockGet.mockResolvedValue({ data: [] });
    await getProductImages(42);
    expect(mockGet).toHaveBeenCalledWith("/productImages/by-product/42");
  });

  it("deleteProductImage DELETEs with 30s timeout", async () => {
    mockDelete.mockResolvedValue({ data: null });
    await deleteProductImage(99);
    expect(mockDelete).toHaveBeenCalledWith(
      "/productImages/99",
      expect.objectContaining({ timeout: 30000 })
    );
  });
});
