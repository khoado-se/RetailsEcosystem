import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockCreate = vi.fn();
const mockUpdate = vi.fn();
const mockToast = { success: vi.fn(), error: vi.fn() };

vi.mock("@features/category/categoryApi", () => ({
  createCategory: mockCreate,
  updateCategory: mockUpdate,
}));
vi.mock("react-hot-toast", () => ({ default: mockToast }));

const { default: CategoryForm } = await import("@features/category/CategoryForm");

function renderCreate(onSuccess = vi.fn(), onClose = vi.fn()) {
  return render(<CategoryForm category={null} onSuccess={onSuccess} onClose={onClose} />);
}

function renderEdit(cat = { id: 1, name: "Old", description: "Desc" }, onSuccess = vi.fn(), onClose = vi.fn()) {
  return render(<CategoryForm category={cat} onSuccess={onSuccess} onClose={onClose} />);
}

describe("CategoryForm — create mode", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders Create Category title", () => {
    renderCreate();
    expect(screen.getByText("Create Category")).toBeInTheDocument();
  });

  it("shows inline error when name is empty on submit", async () => {
    renderCreate();
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() =>
      expect(screen.getByText("Category name is required.")).toBeInTheDocument()
    );
    expect(mockCreate).not.toHaveBeenCalled();
  });

  it("calls createCategory and onSuccess on valid submit", async () => {
    mockCreate.mockResolvedValue({ data: {} });
    const onSuccess = vi.fn();
    renderCreate(onSuccess);
    fireEvent.change(screen.getByLabelText(/Name/i), { target: { value: "Electronics" } });
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() => expect(mockCreate).toHaveBeenCalledWith({
      name: "Electronics",
      description: null,
    }));
    expect(mockToast.success).toHaveBeenCalled();
    expect(onSuccess).toHaveBeenCalled();
  });

  it("calls createCategory with description when provided", async () => {
    mockCreate.mockResolvedValue({ data: {} });
    renderCreate();
    fireEvent.change(screen.getByLabelText(/Name/i), { target: { value: "Books" } });
    fireEvent.change(screen.getByLabelText(/Description/i), { target: { value: "All books" } });
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() =>
      expect(mockCreate).toHaveBeenCalledWith({ name: "Books", description: "All books" })
    );
  });

  it("shows API error message on failure", async () => {
    mockCreate.mockRejectedValue({ response: { data: { title: "Name conflict" } } });
    renderCreate();
    fireEvent.change(screen.getByLabelText(/Name/i), { target: { value: "Dup" } });
    fireEvent.click(screen.getByText("Create"));
    await waitFor(() =>
      expect(screen.getByText("Name conflict")).toBeInTheDocument()
    );
  });

  it("calls onClose when cancel is clicked", () => {
    const onClose = vi.fn();
    renderCreate(vi.fn(), onClose);
    fireEvent.click(screen.getByText("Cancel"));
    expect(onClose).toHaveBeenCalled();
  });

  it("calls onClose when × button is clicked", () => {
    const onClose = vi.fn();
    renderCreate(vi.fn(), onClose);
    fireEvent.click(document.querySelector(".btn-close")!);
    expect(onClose).toHaveBeenCalled();
  });
});

describe("CategoryForm — edit mode", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders Edit Category title", () => {
    renderEdit();
    expect(screen.getByText("Edit Category")).toBeInTheDocument();
  });

  it("pre-fills name and description from category prop", () => {
    renderEdit({ id: 5, name: "Gadgets", description: "Tech gadgets" });
    expect((screen.getByLabelText(/Name/i) as HTMLInputElement).value).toBe("Gadgets");
    expect((screen.getByLabelText(/Description/i) as HTMLTextAreaElement).value).toBe("Tech gadgets");
  });

  it("calls updateCategory on valid submit in edit mode", async () => {
    mockUpdate.mockResolvedValue({ data: {} });
    const onSuccess = vi.fn();
    renderEdit({ id: 5, name: "Old", description: "" }, onSuccess);
    fireEvent.change(screen.getByLabelText(/Name/i), { target: { value: "New Name" } });
    fireEvent.click(screen.getByText("Save changes"));
    await waitFor(() =>
      expect(mockUpdate).toHaveBeenCalledWith(5, { id: 5, name: "New Name", description: null })
    );
    expect(onSuccess).toHaveBeenCalled();
  });
});
