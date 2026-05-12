import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

// --- Mocks ---

const { mockModalInstance, mockCreateProduct, mockUploadProductImage, mockToast } = vi.hoisted(() => ({
  mockModalInstance: { show: vi.fn(), hide: vi.fn() },
  mockCreateProduct: vi.fn(),
  mockUploadProductImage: vi.fn(),
  mockToast: { success: vi.fn(), error: vi.fn() },
}));

vi.mock("bootstrap", () => ({
  Modal: { getOrCreateInstance: vi.fn(() => mockModalInstance) },
}));

vi.mock("@features/product/productApi", () => ({
  createProduct: mockCreateProduct,
}));

vi.mock("@features/productImage/productImageApi", () => ({
  uploadProductImage: mockUploadProductImage,
}));

vi.mock("react-hot-toast", () => ({ default: mockToast }));

// Stub ProductForm so we control form inputs without CategorySelect complexity
vi.mock("@features/product/ProductForm", () => ({
  default: ({ form, handleChange, imageSection }: {
    form: Record<string, unknown>;
    handleChange: (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
    imageSection: React.ReactNode;
  }) => (
    <div className="modal-body">
      <input name="name"        data-testid="input-name"        value={form.name as string}    onChange={handleChange} />
      <input name="price"       data-testid="input-price"       value={form.price as string}   onChange={handleChange} type="number" />
      <input name="categoryId"  data-testid="input-categoryId"  value={form.categoryId as string} onChange={handleChange} />
      <input name="description" data-testid="input-description" value={form.description as string} onChange={handleChange} />
      <input name="isFeatured"  data-testid="input-featured"    checked={!!form.isFeatured}    onChange={handleChange} type="checkbox" />
      {imageSection}
    </div>
  ),
}));

import CreateProductModal from "@features/product/CreateProductModal";

// ---

function renderModal(overrides = {}) {
  const props = {
    isOpen: true,
    onSuccess: vi.fn(),
    onClose: vi.fn(),
    ...overrides,
  };
  return { ...render(<CreateProductModal {...props} />), props };
}

function fillValidForm() {
  fireEvent.change(screen.getByTestId("input-name"),       { target: { name: "name",       value: "Phone" } });
  fireEvent.change(screen.getByTestId("input-price"),      { target: { name: "price",      value: "999" } });
  fireEvent.change(screen.getByTestId("input-categoryId"), { target: { name: "categoryId", value: "2" } });
}

describe("CreateProductModal — Bootstrap modal control", () => {
  beforeEach(() => vi.clearAllMocks());

  it("calls show() when isOpen is true", () => {
    renderModal({ isOpen: true });
    expect(mockModalInstance.show).toHaveBeenCalled();
  });

  it("calls hide() when isOpen is false", () => {
    renderModal({ isOpen: false });
    expect(mockModalInstance.hide).toHaveBeenCalled();
  });
});

describe("CreateProductModal — form validation", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows validation error and does not call createProduct when name is empty", async () => {
    renderModal();
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() => expect(mockToast.error).toHaveBeenCalledWith("Product name is required."));
    expect(mockCreateProduct).not.toHaveBeenCalled();
  });

  it("shows validation error when price is zero", async () => {
    renderModal();
    fireEvent.change(screen.getByTestId("input-name"),  { target: { name: "name",  value: "Phone" } });
    fireEvent.change(screen.getByTestId("input-price"), { target: { name: "price", value: "0" } });
    fireEvent.change(screen.getByTestId("input-categoryId"), { target: { name: "categoryId", value: "2" } });
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() => expect(mockToast.error).toHaveBeenCalledWith("Price must be greater than 0."));
    expect(mockCreateProduct).not.toHaveBeenCalled();
  });

  it("shows validation error when categoryId is empty", async () => {
    renderModal();
    fireEvent.change(screen.getByTestId("input-name"),  { target: { name: "name",  value: "Phone" } });
    fireEvent.change(screen.getByTestId("input-price"), { target: { name: "price", value: "999" } });
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() => expect(mockToast.error).toHaveBeenCalledWith("Please select a category."));
    expect(mockCreateProduct).not.toHaveBeenCalled();
  });
});

describe("CreateProductModal — successful submit (no files)", () => {
  beforeEach(() => vi.clearAllMocks());

  it("calls createProduct with correct payload", async () => {
    mockCreateProduct.mockResolvedValue({ data: { id: 42 } });
    renderModal();
    fillValidForm();
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() => expect(mockCreateProduct).toHaveBeenCalledWith({
      name: "Phone",
      description: null,
      price: 999,
      categoryId: 2,
      isFeatured: false,
    }));
  });

  it("calls onSuccess and onClose after successful create", async () => {
    mockCreateProduct.mockResolvedValue({ data: { id: 42 } });
    const { props } = renderModal();
    fillValidForm();
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() => {
      expect(props.onSuccess).toHaveBeenCalledTimes(1);
      expect(props.onClose).toHaveBeenCalledTimes(1);
    });
  });

  it("shows success toast after product created", async () => {
    mockCreateProduct.mockResolvedValue({ data: { id: 42 } });
    renderModal();
    fillValidForm();
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() =>
      expect(mockToast.success).toHaveBeenCalledWith("Product created successfully.")
    );
  });

  it("does not call uploadProductImage when no files selected", async () => {
    mockCreateProduct.mockResolvedValue({ data: { id: 42 } });
    const { props } = renderModal();
    fillValidForm();
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() => expect(props.onSuccess).toHaveBeenCalled());
    expect(mockUploadProductImage).not.toHaveBeenCalled();
  });
});

describe("CreateProductModal — createProduct failure", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows error toast and does not call onSuccess when API rejects", async () => {
    mockCreateProduct.mockRejectedValue(new Error("Server error"));
    const { props } = renderModal();
    fillValidForm();
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() =>
      expect(mockToast.error).toHaveBeenCalledWith("Failed to create product.")
    );
    expect(props.onSuccess).not.toHaveBeenCalled();
    expect(props.onClose).not.toHaveBeenCalled();
  });
});

describe("CreateProductModal — file upload", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.stubGlobal("URL", {
      createObjectURL: vi.fn(() => "blob:fake"),
      revokeObjectURL: vi.fn(),
    });
  });

  it("rejects unsupported file type and shows toast.error", async () => {
    renderModal();
    const fileInput = screen.getByTestId("file-input");
    const badFile = new File(["data"], "doc.pdf", { type: "application/pdf" });
    fireEvent.change(fileInput, { target: { files: [badFile] } });
    expect(mockToast.error).toHaveBeenCalledWith(expect.stringContaining("doc.pdf"));
  });

  it("calls uploadProductImage after successful create when valid files selected", async () => {
    mockCreateProduct.mockResolvedValue({ data: { id: 42 } });
    mockUploadProductImage.mockResolvedValue(undefined);
    const { props } = renderModal();

    const fileInput = screen.getByTestId("file-input");
    const validFile = new File(["img"], "photo.jpg", { type: "image/jpeg" });
    fireEvent.change(fileInput, { target: { files: [validFile] } });

    fillValidForm();
    fireEvent.click(screen.getByText("Create"));

    await waitFor(() => expect(props.onSuccess).toHaveBeenCalled());
    await waitFor(() => expect(mockUploadProductImage).toHaveBeenCalledWith(42, expect.any(FormData)));
  });

  it("shows upload error toast when uploadProductImage rejects", async () => {
    mockCreateProduct.mockResolvedValue({ data: { id: 42 } });
    mockUploadProductImage.mockRejectedValue(new Error("upload failed"));
    renderModal();

    const fileInput = screen.getByTestId("file-input");
    const validFile = new File(["img"], "photo.jpg", { type: "image/jpeg" });
    fireEvent.change(fileInput, { target: { files: [validFile] } });

    fillValidForm();
    fireEvent.click(screen.getByText("Create"));

    await waitFor(() =>
      expect(mockToast.error).toHaveBeenCalledWith(
        "Product created, but images could not be uploaded."
      )
    );
  });
});
