import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockModalInstance = { show: vi.fn(), hide: vi.fn() };
const mockGetImages = vi.fn();
const mockUpload = vi.fn();
const mockDelete = vi.fn();
const mockToast = { success: vi.fn(), error: vi.fn() };

vi.mock("bootstrap", () => ({ Modal: { getOrCreateInstance: vi.fn(() => mockModalInstance) } }));
vi.mock("@features/productImage/productImageApi", () => ({
  getProductImages: mockGetImages,
  uploadProductImage: mockUpload,
  deleteProductImage: mockDelete,
}));
vi.mock("react-hot-toast", () => ({ default: mockToast }));

const { default: ProductImageModal } = await import("@features/productImage/ProductImageModal");

const IMAGES = [{ id: 10, url: "/img/1.jpg" }, { id: 11, url: "/img/2.jpg" }];

function renderModal(productId: number | null = 5, onClose = vi.fn(), onChanged = vi.fn()) {
  return { ...render(<ProductImageModal productId={productId} productName="Phone" onClose={onClose} onChanged={onChanged} />), onClose, onChanged };
}

describe("ProductImageModal — show/hide", () => {
  beforeEach(() => { vi.clearAllMocks(); mockGetImages.mockResolvedValue({ data: IMAGES }); });

  it("calls show() when productId is truthy", async () => {
    renderModal(5);
    await waitFor(() => expect(mockGetImages).toHaveBeenCalledWith(5));
    expect(mockModalInstance.show).toHaveBeenCalled();
  });

  it("calls hide() when productId is null", () => {
    renderModal(null);
    expect(mockModalInstance.hide).toHaveBeenCalled();
  });
});

describe("ProductImageModal — image list", () => {
  beforeEach(() => { vi.clearAllMocks(); mockGetImages.mockResolvedValue({ data: IMAGES }); });

  it("renders existing images", async () => {
    renderModal(5);
    // Images use alt="" so they have presentation role; query by tag directly
    await waitFor(() => {
      const imgs = document.querySelectorAll("img");
      expect(imgs.length).toBe(2);
    });
  });

  it("shows error message when getProductImages fails", async () => {
    mockGetImages.mockRejectedValue(new Error("fail"));
    renderModal(5);
    await waitFor(() => expect(screen.getByText("Failed to load images.")).toBeInTheDocument());
  });

  it("shows 'No images yet.' when images list is empty", async () => {
    mockGetImages.mockResolvedValue({ data: [] });
    renderModal(5);
    await waitFor(() => expect(screen.getByText("No images yet.")).toBeInTheDocument());
  });
});

describe("ProductImageModal — delete image", () => {
  beforeEach(() => { vi.clearAllMocks(); mockGetImages.mockResolvedValue({ data: IMAGES }); });

  it("removes image from UI immediately on delete click", async () => {
    mockDelete.mockResolvedValue({});
    renderModal(5);
    await waitFor(() => expect(screen.getAllByTitle("Delete image").length).toBe(2));
    fireEvent.click(screen.getAllByTitle("Delete image")[0]);
    const imgs = document.querySelectorAll("img");
    expect(imgs.length).toBe(1);
  });

  it("calls deleteProductImage and onChanged on success", async () => {
    mockDelete.mockResolvedValue({});
    const { onChanged } = renderModal(5);
    await waitFor(() => screen.getAllByTitle("Delete image"));
    fireEvent.click(screen.getAllByTitle("Delete image")[0]);
    await waitFor(() => expect(mockDelete).toHaveBeenCalledWith(10));
    expect(onChanged).toHaveBeenCalled();
    expect(mockToast.success).toHaveBeenCalled();
  });

  it("shows error and restores images on delete failure", async () => {
    mockDelete.mockRejectedValue(new Error("fail"));
    mockGetImages.mockResolvedValueOnce({ data: IMAGES }).mockResolvedValue({ data: IMAGES });
    renderModal(5);
    await waitFor(() => screen.getAllByTitle("Delete image"));
    fireEvent.click(screen.getAllByTitle("Delete image")[0]);
    await waitFor(() => expect(screen.getByText("Delete failed. Image restored.")).toBeInTheDocument());
  });
});

describe("ProductImageModal — file upload", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockGetImages.mockResolvedValue({ data: [] });
    vi.stubGlobal("URL", { createObjectURL: vi.fn(() => "blob:fake"), revokeObjectURL: vi.fn() });
  });

  it("rejects unsupported file types", async () => {
    renderModal(5);
    await waitFor(() => screen.getByText("No images yet."));
    const input = document.querySelector("input[type=file]") as HTMLInputElement;
    const bad = new File(["x"], "doc.pdf", { type: "application/pdf" });
    fireEvent.change(input, { target: { files: [bad] } });
    expect(screen.getByText(/Unsupported file type/)).toBeInTheDocument();
  });

  it("Upload button is disabled when no files selected", async () => {
    renderModal(5);
    await waitFor(() => screen.getByText("No images yet."));
    // Find the upload button by its class (btn-primary btn-sm) to avoid matching description text
    const uploadBtn = document.querySelector("button.btn-primary.btn-sm") as HTMLButtonElement;
    expect(uploadBtn).toBeDisabled();
  });

  it("upload button shows file count when files selected", async () => {
    renderModal(5);
    await waitFor(() => screen.getByText("No images yet."));
    const input = document.querySelector("input[type=file]") as HTMLInputElement;
    const validFile = new File(["x"], "photo.jpg", { type: "image/jpeg" });
    fireEvent.change(input, { target: { files: [validFile] } });
    expect(screen.getByText(/1 file/)).toBeInTheDocument();
  });

  it("calls uploadProductImage and shows toast on upload success", async () => {
    mockUpload.mockResolvedValue({});
    mockGetImages.mockResolvedValue({ data: [] });
    renderModal(5);
    await waitFor(() => screen.getByText("No images yet."));
    const input = document.querySelector("input[type=file]") as HTMLInputElement;
    const validFile = new File(["x"], "photo.jpg", { type: "image/jpeg" });
    fireEvent.change(input, { target: { files: [validFile] } });
    const uploadBtn = screen.getByText(/1 file/).closest("button");
    fireEvent.click(uploadBtn!);
    await waitFor(() => expect(mockUpload).toHaveBeenCalledWith(5, expect.any(FormData)));
    expect(mockToast.success).toHaveBeenCalled();
  });

  it("shows upload error on failure", async () => {
    mockUpload.mockRejectedValue(new Error("fail"));
    renderModal(5);
    await waitFor(() => screen.getByText("No images yet."));
    const input = document.querySelector("input[type=file]") as HTMLInputElement;
    const validFile = new File(["x"], "photo.jpg", { type: "image/jpeg" });
    fireEvent.change(input, { target: { files: [validFile] } });
    const uploadBtn = screen.getByText(/1 file/).closest("button");
    fireEvent.click(uploadBtn!);
    await waitFor(() => expect(screen.getByText("Upload failed. Please try again.")).toBeInTheDocument());
  });
});
