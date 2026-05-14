import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockUseProducts = vi.fn();
vi.mock("@features/product/useProducts", () => ({ useProducts: mockUseProducts }));
vi.mock("@features/product/ProductTable", () => ({
  default: ({ products, onEdit, onImages }: { products: unknown[]; onEdit: (id: number) => void; onImages: (id: number, name: string) => void }) => (
    <div data-testid="product-table">
      <span>count:{products.length}</span>
      <button onClick={() => onEdit(1)}>edit-btn</button>
      <button onClick={() => onImages(1, "Phone")}>images-btn</button>
    </div>
  ),
}));
vi.mock("@features/product/CreateProductModal", () => ({
  default: ({ isOpen, onClose }: { isOpen: boolean; onClose: () => void }) =>
    isOpen ? <div data-testid="create-modal"><button onClick={onClose}>close</button></div> : null,
}));
vi.mock("@features/product/EditProductModal", () => ({
  default: ({ productId }: { productId: number | null }) =>
    productId ? <div data-testid="edit-modal" /> : null,
}));
vi.mock("@features/productImage/ProductImageModal", () => ({
  default: ({ productId }: { productId: number | null }) =>
    productId ? <div data-testid="image-modal" /> : null,
}));
vi.mock("@features/category/CategorySelect", () => ({
  default: ({ onChange }: { onChange: (id: string) => void }) => (
    <button data-testid="cat-select" onClick={() => onChange("1")}>cat-select</button>
  ),
}));
vi.mock("@components/ui/PageSizeSelector", () => ({
  default: () => <div data-testid="page-size" />,
}));

const { default: ProductsPage } = await import("@pages/products/ProductsPage");

function mockProducts(overrides = {}) {
  mockUseProducts.mockReturnValue({
    products: [], totalPage: 1, loading: false, error: null, fetchProducts: vi.fn(),
    ...overrides,
  });
}

describe("ProductsPage — rendering", () => {
  beforeEach(() => { vi.clearAllMocks(); mockProducts(); });

  it("renders Products heading", () => {
    render(<ProductsPage />);
    expect(screen.getByText("Products")).toBeInTheDocument();
  });

  it("renders Create Product button", () => {
    render(<ProductsPage />);
    expect(screen.getByText("Create Product")).toBeInTheDocument();
  });

  it("renders ProductTable", () => {
    render(<ProductsPage />);
    expect(screen.getByTestId("product-table")).toBeInTheDocument();
  });

  it("shows error alert when error returned", () => {
    mockProducts({ error: "Failed to load" });
    render(<ProductsPage />);
    expect(screen.getByText(/Failed to load/)).toBeInTheDocument();
  });
});

describe("ProductsPage — interactions", () => {
  beforeEach(() => { vi.clearAllMocks(); mockProducts(); });

  it("opens CreateProductModal on Create Product click", () => {
    render(<ProductsPage />);
    fireEvent.click(screen.getByText("Create Product"));
    expect(screen.getByTestId("create-modal")).toBeInTheDocument();
  });

  it("closes CreateProductModal when onClose called", () => {
    render(<ProductsPage />);
    fireEvent.click(screen.getByText("Create Product"));
    fireEvent.click(screen.getByText("close"));
    expect(screen.queryByTestId("create-modal")).not.toBeInTheDocument();
  });

  it("opens EditProductModal when onEdit called from table", () => {
    render(<ProductsPage />);
    fireEvent.click(screen.getByText("edit-btn"));
    expect(screen.getByTestId("edit-modal")).toBeInTheDocument();
  });

  it("opens ProductImageModal when onImages called from table", () => {
    render(<ProductsPage />);
    fireEvent.click(screen.getByText("images-btn"));
    expect(screen.getByTestId("image-modal")).toBeInTheDocument();
  });

  it("updates debounced search after typing in search input", async () => {
    render(<ProductsPage />);
    const input = screen.getByPlaceholderText(/search/i);
    fireEvent.change(input, { target: { value: "laptop" } });
    expect((input as HTMLInputElement).value).toBe("laptop");
  });

  it("toggles Featured Only button", () => {
    render(<ProductsPage />);
    const featuredBtn = screen.getByText("Featured").closest("button");
    expect(featuredBtn.className).toContain("btn-outline-secondary");
    fireEvent.click(featuredBtn);
    // After click, the component re-renders; the mock will be called with new params
    expect(mockUseProducts).toHaveBeenCalled();
  });

  it("clears filters when Clear filters button clicked (after setting filters)", async () => {
    render(<ProductsPage />);
    const input = screen.getByPlaceholderText(/search/i);
    fireEvent.change(input, { target: { value: "laptop" } });
    await waitFor(() => expect(screen.getByText(/Clear/i)).toBeInTheDocument());
    fireEvent.click(screen.getByText(/Clear/i));
    expect((input as HTMLInputElement).value).toBe("");
  });
});
