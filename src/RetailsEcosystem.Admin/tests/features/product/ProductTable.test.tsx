import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

// --- Mocks (hoisted before imports) ---

const { mockDeleteProduct, mockToast } = vi.hoisted(() => ({
  mockDeleteProduct: vi.fn(),
  mockToast: { success: vi.fn(), error: vi.fn() },
}));

vi.mock("@features/product/productApi", () => ({
  deleteProduct: mockDeleteProduct,
}));

vi.mock("react-hot-toast", () => ({ default: mockToast }));

vi.mock("@configs/env", () => ({ ENV: { PRODUCT_PLACEHOLDER_IMAGE: "/placeholder.jpg" } }));

// Mock ConfirmModal so Bootstrap JS is not needed; render inline buttons
vi.mock("@components/ui/ConfirmModal", () => ({
  default: ({ title, message, confirmLabel, onConfirm, onClose }: {
    title: string;
    message: string;
    confirmLabel: string;
    onConfirm: () => void;
    onClose: () => void;
  }) => (
    <div data-testid="confirm-modal">
      <span data-testid="modal-title">{title}</span>
      <span data-testid="modal-message">{message}</span>
      <button data-testid="modal-confirm" onClick={onConfirm}>{confirmLabel}</button>
      <button data-testid="modal-cancel" onClick={onClose}>Cancel</button>
    </div>
  ),
}));

// Mock Pagination to keep tests focused on ProductTable
vi.mock("@components/ui/Pagination", () => ({
  default: () => <div data-testid="pagination" />,
}));

import ProductTable from "@features/product/ProductTable";

// ---

const PRODUCTS = [
  { id: 1, name: "Phone", price: 999, isFeatured: true,  imageUrl: "/phone.jpg", createdDate: "2024-01-01" },
  { id: 2, name: "Laptop", price: 1499, isFeatured: false, imageUrl: null,        createdDate: "2024-02-01" },
];

function renderTable(overrides = {}) {
  const props = {
    products: PRODUCTS,
    pageNumber: 1,
    setPageNumber: vi.fn(),
    totalPage: 1,
    loading: false,
    onEdit: vi.fn(),
    onDeleted: vi.fn(),
    onImages: vi.fn(),
    onClearFilters: vi.fn(),
    sortBy: "id",
    sortDesc: true,
    onSort: vi.fn(),
    ...overrides,
  };
  return { ...render(<ProductTable {...props} />), props };
}

describe("ProductTable — product list", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders one row per product with name and price", () => {
    renderTable();
    expect(screen.getByText("Phone")).toBeInTheDocument();
    expect(screen.getByText("Laptop")).toBeInTheDocument();
    expect(screen.getByText("999")).toBeInTheDocument();
    expect(screen.getByText("1499")).toBeInTheDocument();
  });

  it("renders Featured badge for isFeatured products only", () => {
    renderTable();
    const badges = screen.getAllByText("Featured").filter(el => el.tagName === "SPAN");
    expect(badges).toHaveLength(1);
  });

  it("uses placeholder image when product has no imageUrl", () => {
    renderTable();
    const imgs = screen.getAllByAltText("Product") as HTMLImageElement[];
    const laptopImg = imgs.find((img) => img.src.includes("placeholder"));
    expect(laptopImg).toBeDefined();
  });

  it("fires onEdit with product id when Edit clicked", () => {
    const { props } = renderTable();
    const editBtns = screen.getAllByTitle("Edit");
    fireEvent.click(editBtns[0]);
    expect(props.onEdit).toHaveBeenCalledWith(1);
  });

  it("fires onImages with product id and name when Images clicked", () => {
    const { props } = renderTable();
    const imageBtns = screen.getAllByTitle("Images");
    fireEvent.click(imageBtns[0]);
    expect(props.onImages).toHaveBeenCalledWith(1, "Phone");
  });
});

describe("ProductTable — empty state", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders No products found when products is empty", () => {
    renderTable({ products: [], loading: false });
    expect(screen.getByText("No products found")).toBeInTheDocument();
  });

  it("shows Clear filters button when onClearFilters provided", () => {
    renderTable({ products: [], loading: false });
    expect(screen.getByText("Clear filters")).toBeInTheDocument();
  });

  it("hides Clear filters when onClearFilters is undefined", () => {
    renderTable({ products: [], loading: false, onClearFilters: undefined });
    expect(screen.queryByText("Clear filters")).not.toBeInTheDocument();
  });

  it("does not show empty state while loading even with empty products", () => {
    renderTable({ products: [], loading: true });
    expect(screen.queryByText("No products found")).not.toBeInTheDocument();
  });
});

describe("ProductTable — delete flow", () => {
  beforeEach(() => vi.clearAllMocks());

  it("opens ConfirmModal with product name when Delete clicked", () => {
    renderTable();
    const deleteBtns = screen.getAllByTitle("Delete");
    fireEvent.click(deleteBtns[0]);
    expect(screen.getByTestId("confirm-modal")).toBeInTheDocument();
    expect(screen.getByTestId("modal-message").textContent).toContain("Phone");
  });

  it("calls deleteProduct and onDeleted on confirm", async () => {
    mockDeleteProduct.mockResolvedValue(undefined);
    const { props } = renderTable();

    fireEvent.click(screen.getAllByTitle("Delete")[0]);
    fireEvent.click(screen.getByTestId("modal-confirm"));

    await waitFor(() => expect(mockDeleteProduct).toHaveBeenCalledWith(1));
    expect(props.onDeleted).toHaveBeenCalledTimes(1);
    expect(mockToast.success).toHaveBeenCalledWith("Product deleted.");
  });

  it("shows toast.error and does not call onDeleted when delete API fails", async () => {
    mockDeleteProduct.mockRejectedValue(new Error("Server error"));
    const { props } = renderTable();

    fireEvent.click(screen.getAllByTitle("Delete")[0]);
    fireEvent.click(screen.getByTestId("modal-confirm"));

    await waitFor(() => expect(mockToast.error).toHaveBeenCalledWith("Failed to delete product."));
    expect(props.onDeleted).not.toHaveBeenCalled();
  });

  it("closes ConfirmModal when cancel clicked without calling deleteProduct", () => {
    renderTable();
    fireEvent.click(screen.getAllByTitle("Delete")[0]);
    expect(screen.getByTestId("confirm-modal")).toBeInTheDocument();

    fireEvent.click(screen.getByTestId("modal-cancel"));
    expect(screen.queryByTestId("confirm-modal")).not.toBeInTheDocument();
    expect(mockDeleteProduct).not.toHaveBeenCalled();
  });
});

describe("ProductTable — sorting", () => {
  beforeEach(() => vi.clearAllMocks());

  it("calls onSort with column name when sort header clicked", () => {
    const onSort = vi.fn();
    renderTable({ onSort });
    fireEvent.click(screen.getByText("Name"));
    expect(onSort).toHaveBeenCalledWith("name");
  });

  it("shows down-arrow icon on active column when sortDesc=true", () => {
    renderTable({ sortBy: "name", sortDesc: true });
    const nameHeader = screen.getByText("Name").closest("th");
    expect(nameHeader?.querySelector(".bi-arrow-down")).toBeTruthy();
  });

  it("shows up-arrow icon on active column when sortDesc=false", () => {
    renderTable({ sortBy: "name", sortDesc: false });
    const nameHeader = screen.getByText("Name").closest("th");
    expect(nameHeader?.querySelector(".bi-arrow-up")).toBeTruthy();
  });

  it("shows neutral sort icon on inactive column", () => {
    renderTable({ sortBy: "id", sortDesc: true });
    const nameHeader = screen.getByText("Name").closest("th");
    expect(nameHeader?.querySelector(".bi-arrow-down-up")).toBeTruthy();
  });
});
