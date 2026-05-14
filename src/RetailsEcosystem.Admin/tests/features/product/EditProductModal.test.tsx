import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockModalInstance = { show: vi.fn(), hide: vi.fn() };
const mockGetProductById = vi.fn();
const mockUpdateProduct = vi.fn();
const mockGetProductImages = vi.fn();
const mockUploadProductImage = vi.fn();
const mockDeleteProductImage = vi.fn();
const mockToast = { success: vi.fn(), error: vi.fn() };

vi.mock("bootstrap", () => ({ Modal: { getOrCreateInstance: vi.fn(() => mockModalInstance) } }));
vi.mock("@features/product/productApi", () => ({
  getProductById: mockGetProductById,
  updateProduct: mockUpdateProduct,
}));
vi.mock("@features/productImage/productImageApi", () => ({
  getProductImages: mockGetProductImages,
  uploadProductImage: mockUploadProductImage,
  deleteProductImage: mockDeleteProductImage,
}));
vi.mock("react-hot-toast", () => ({ default: mockToast }));
vi.mock("@features/product/ProductForm", () => ({
  default: ({ form, handleChange, imageSection }: {
    form: Record<string, unknown>;
    handleChange: (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
    imageSection: React.ReactNode;
  }) => (
    <div className="modal-body">
      <input name="name" data-testid="input-name" value={form.name as string} onChange={handleChange} />
      <input name="price" data-testid="input-price" value={form.price as string} onChange={handleChange} type="number" />
      <input name="categoryId" data-testid="input-cat" value={form.categoryId as string} onChange={handleChange} />
      <input name="description" data-testid="input-desc" value={form.description as string} onChange={handleChange} />
      <input name="isFeatured" data-testid="input-featured" type="checkbox" checked={!!form.isFeatured} onChange={handleChange} />
      {imageSection}
    </div>
  ),
}));

const { default: EditProductModal } = await import("@features/product/EditProductModal");

const PRODUCT = { id: 5, name: "Phone", description: "Desc", price: 999, isFeatured: false, category: { id: 1, name: "Electronics" } };
const IMAGES = [{ id: 10, url: "/img/1.jpg" }];

function renderModal(productId: number | null = 5, overrides = {}) {
  const props = { productId, onSuccess: vi.fn(), onClose: vi.fn(), ...overrides };
  return { ...render(<EditProductModal {...props} />), props };
}

describe("EditProductModal — show/hide", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockGetProductById.mockResolvedValue({ data: PRODUCT });
    mockGetProductImages.mockResolvedValue({ data: IMAGES });
  });

  it("calls show() when productId is truthy", async () => {
    renderModal(5);
    await waitFor(() => expect(mockGetProductById).toHaveBeenCalledWith(5));
    expect(mockModalInstance.show).toHaveBeenCalled();
  });

  it("calls hide() when productId is null", () => {
    renderModal(null);
    expect(mockModalInstance.hide).toHaveBeenCalled();
  });

  it("shows Loading... placeholder before product loads", () => {
    mockGetProductById.mockReturnValue(new Promise(() => {}));
    mockGetProductImages.mockReturnValue(new Promise(() => {}));
    renderModal(5);
    expect(screen.getByText("Loading…")).toBeInTheDocument();
  });
});

describe("EditProductModal — form load and submit", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockGetProductById.mockResolvedValue({ data: PRODUCT });
    mockGetProductImages.mockResolvedValue({ data: IMAGES });
  });

  it("populates form with product data", async () => {
    renderModal(5);
    await waitFor(() => expect((screen.getByTestId("input-name") as HTMLInputElement).value).toBe("Phone"));
    expect((screen.getByTestId("input-price") as HTMLInputElement).value).toBe("999");
  });

  it("shows validation error toast when name is empty", async () => {
    renderModal(5);
    await waitFor(() => expect(screen.getByTestId("input-name")).toBeInTheDocument());
    fireEvent.change(screen.getByTestId("input-name"), { target: { name: "name", value: "" } });
    fireEvent.click(screen.getByText("Save Changes"));
    await waitFor(() => expect(mockToast.error).toHaveBeenCalledWith("Product name is required."));
    expect(mockUpdateProduct).not.toHaveBeenCalled();
  });

  it("calls updateProduct with correct payload on valid submit", async () => {
    mockUpdateProduct.mockResolvedValue({});
    renderModal(5);
    await waitFor(() => expect(screen.getByTestId("input-name")).toBeInTheDocument());
    fireEvent.click(screen.getByText("Save Changes"));
    await waitFor(() =>
      expect(mockUpdateProduct).toHaveBeenCalledWith(5, expect.objectContaining({ name: "Phone", price: 999 }))
    );
    expect(mockToast.success).toHaveBeenCalledWith("Product updated successfully.");
  });

  it("shows error toast when updateProduct fails", async () => {
    mockUpdateProduct.mockRejectedValue(new Error("fail"));
    renderModal(5);
    await waitFor(() => expect(screen.getByTestId("input-name")).toBeInTheDocument());
    fireEvent.click(screen.getByText("Save Changes"));
    await waitFor(() => expect(mockToast.error).toHaveBeenCalledWith("Failed to update product."));
  });
});

describe("EditProductModal — existing images", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockGetProductById.mockResolvedValue({ data: PRODUCT });
    mockGetProductImages.mockResolvedValue({ data: IMAGES });
    mockUpdateProduct.mockResolvedValue({});
  });

  it("renders existing image thumbnails", async () => {
    renderModal(5);
    await waitFor(() => expect(document.querySelectorAll("img").length).toBeGreaterThan(0));
  });

  it("marks image for delete when × clicked", async () => {
    renderModal(5);
    await waitFor(() => expect(screen.getAllByTitle("Remove image").length).toBeGreaterThan(0));
    fireEvent.click(screen.getAllByTitle("Remove image")[0]);
    expect(screen.queryAllByTitle("Remove image")).toHaveLength(0);
  });
});

describe("EditProductModal — file upload", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockGetProductById.mockResolvedValue({ data: PRODUCT });
    mockGetProductImages.mockResolvedValue({ data: [] });
    vi.stubGlobal("URL", { createObjectURL: vi.fn(() => "blob:fake"), revokeObjectURL: vi.fn() });
  });

  it("rejects unsupported file types with toast.error", async () => {
    renderModal(5);
    await waitFor(() => expect(screen.getByTestId("input-name")).toBeInTheDocument());
    const fileInput = document.querySelector("input[type=file]") as HTMLInputElement;
    const badFile = new File(["x"], "doc.pdf", { type: "application/pdf" });
    fireEvent.change(fileInput, { target: { files: [badFile] } });
    expect(mockToast.error).toHaveBeenCalledWith(expect.stringContaining("doc.pdf"));
  });
});
