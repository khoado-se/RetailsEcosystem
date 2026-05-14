import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockUseCategories = vi.fn();
const mockDeleteCategory = vi.fn();
const mockToast = { success: vi.fn(), error: vi.fn() };

vi.mock("@features/category/useCategories", () => ({ useCategories: mockUseCategories }));
vi.mock("@features/category/categoryApi", () => ({ deleteCategory: mockDeleteCategory }));
vi.mock("react-hot-toast", () => ({ default: mockToast }));
vi.mock("@features/category/CategoryTable", () => ({
  default: ({ categories, onEdit, onDelete }: { categories: { id: number; name: string }[]; onEdit: (c: unknown) => void; onDelete: (id: number) => void }) => (
    <div data-testid="cat-table">
      {categories.map((c) => (
        <div key={c.id}>
          <span>{c.name}</span>
          <button onClick={() => onEdit(c)}>edit-{c.id}</button>
          <button onClick={() => onDelete(c.id)}>delete-{c.id}</button>
        </div>
      ))}
    </div>
  ),
}));
vi.mock("@features/category/CategoryForm", () => ({
  default: ({ category, onSuccess, onClose }: { category: unknown; onSuccess: () => void; onClose: () => void }) => (
    <div data-testid="cat-form">
      <button onClick={onSuccess}>form-success</button>
      <button onClick={onClose}>form-close</button>
    </div>
  ),
}));
vi.mock("@components/ui/Pagination", () => ({ default: () => <div data-testid="pagination" /> }));
vi.mock("@components/ui/PageSizeSelector", () => ({ default: () => <div data-testid="page-size" /> }));
vi.mock("@components/ui/ConfirmModal", () => ({
  default: ({ onConfirm, onClose }: { onConfirm: () => void; onClose: () => void }) => (
    <div data-testid="confirm-modal">
      <button onClick={onConfirm}>confirm</button>
      <button onClick={onClose}>cancel</button>
    </div>
  ),
}));

const CATS = [{ id: 1, name: "Electronics" }, { id: 2, name: "Books" }];

function mockCats(overrides = {}) {
  mockUseCategories.mockReturnValue({
    categories: CATS, totalPage: 1, loading: false, error: null, fetchCategories: vi.fn(),
    ...overrides,
  });
}

const { default: CategoriesPage } = await import("@pages/categories/CategoriesPage");

describe("CategoriesPage — rendering", () => {
  beforeEach(() => { vi.clearAllMocks(); mockCats(); });

  it("renders Categories heading", () => {
    render(<CategoriesPage />);
    expect(screen.getByText("Categories")).toBeInTheDocument();
  });

  it("renders category names via CategoryTable", () => {
    render(<CategoriesPage />);
    expect(screen.getByText("Electronics")).toBeInTheDocument();
    expect(screen.getByText("Books")).toBeInTheDocument();
  });

  it("shows error when useCategories returns error", () => {
    mockCats({ error: "Load failed" });
    render(<CategoriesPage />);
    expect(screen.getByText("Load failed")).toBeInTheDocument();
  });
});

describe("CategoriesPage — interactions", () => {
  beforeEach(() => { vi.clearAllMocks(); mockCats(); });

  it("opens CategoryForm (create mode) on Create Category click", () => {
    render(<CategoriesPage />);
    fireEvent.click(screen.getByText(/Create Category/i));
    expect(screen.getByTestId("cat-form")).toBeInTheDocument();
  });

  it("opens CategoryForm (edit mode) on Edit click", () => {
    render(<CategoriesPage />);
    fireEvent.click(screen.getByText("edit-1"));
    expect(screen.getByTestId("cat-form")).toBeInTheDocument();
  });

  it("closes CategoryForm when form-close clicked", () => {
    render(<CategoriesPage />);
    fireEvent.click(screen.getByText(/Create Category/i));
    fireEvent.click(screen.getByText("form-close"));
    expect(screen.queryByTestId("cat-form")).not.toBeInTheDocument();
  });

  it("opens ConfirmModal on Delete click", () => {
    render(<CategoriesPage />);
    fireEvent.click(screen.getByText("delete-1"));
    expect(screen.getByTestId("confirm-modal")).toBeInTheDocument();
  });

  it("calls deleteCategory and toast.success on confirm delete", async () => {
    mockDeleteCategory.mockResolvedValue({});
    render(<CategoriesPage />);
    fireEvent.click(screen.getByText("delete-1"));
    fireEvent.click(screen.getByText("confirm"));
    await waitFor(() => expect(mockDeleteCategory).toHaveBeenCalledWith(1));
    expect(mockToast.success).toHaveBeenCalled();
  });

  it("shows toast.error on delete failure", async () => {
    mockDeleteCategory.mockRejectedValue(new Error("fail"));
    render(<CategoriesPage />);
    fireEvent.click(screen.getByText("delete-1"));
    fireEvent.click(screen.getByText("confirm"));
    await waitFor(() => expect(mockToast.error).toHaveBeenCalled());
  });

  it("filters categories by search term", () => {
    render(<CategoriesPage />);
    const searchInput = screen.getByPlaceholderText(/search/i);
    fireEvent.change(searchInput, { target: { value: "Elec" } });
    expect(screen.getByText("Electronics")).toBeInTheDocument();
    expect(screen.queryByText("Books")).not.toBeInTheDocument();
  });
});
