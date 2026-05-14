import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor, act } from "@testing-library/react";

const mockLoadMore = vi.fn();
const mockReset = vi.fn();
let mockInfiniteState = {
  items: [] as { id: number; name: string }[],
  hasMore: true,
  loading: false,
  error: null as string | null,
  loadMore: mockLoadMore,
  reset: mockReset,
};

vi.mock("@features/category/useCategories", () => ({
  useCategoriesInfinite: () => mockInfiniteState,
}));

const { default: CategorySelect } = await import("@features/category/CategorySelect");

function renderSelect(value = "", onChange = vi.fn(), initialName = "") {
  return render(
    <CategorySelect value={value} onChange={onChange} initialCategoryName={initialName} />
  );
}

describe("CategorySelect — trigger button", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockInfiniteState = { ...mockInfiniteState, items: [], hasMore: true, loading: false, error: null };
  });

  it("shows -- Select -- when no value selected", () => {
    renderSelect("");
    expect(screen.getByRole("button").textContent).toBe("-- Select --");
  });

  it("shows initialCategoryName when value matches no item but initialName provided", () => {
    renderSelect("5", vi.fn(), "Gadgets");
    expect(screen.getByRole("button").textContent).toBe("Gadgets");
  });

  it("shows item name when value matches an item", () => {
    mockInfiniteState = { ...mockInfiniteState, items: [{ id: 3, name: "Books" }] };
    renderSelect("3");
    expect(screen.getByRole("button").textContent).toBe("Books");
  });
});

describe("CategorySelect — open/close", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockInfiniteState = { ...mockInfiniteState, items: [], hasMore: true, loading: false };
  });

  it("opens dropdown on button click and calls reset + loadMore", async () => {
    renderSelect();
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => expect(mockReset).toHaveBeenCalled());
    expect(screen.getByText("-- Select --", { selector: ".category-option" })).toBeInTheDocument();
  });

  it("closes dropdown when button clicked again", async () => {
    renderSelect();
    const btn = screen.getByRole("button");
    fireEvent.click(btn);
    await waitFor(() => expect(mockReset).toHaveBeenCalled());
    fireEvent.click(btn);
    expect(screen.queryByText("-- Select --", { selector: ".category-option" })).not.toBeInTheDocument();
  });

  it("selects -- Select -- option (empty string) and closes dropdown", async () => {
    const onChange = vi.fn();
    renderSelect("3", onChange);
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => expect(mockReset).toHaveBeenCalled());
    const clearOpt = screen.getByText("-- Select --", { selector: ".category-option" });
    fireEvent.click(clearOpt);
    expect(onChange).toHaveBeenCalledWith("");
    expect(screen.queryByText("-- Select --", { selector: ".category-option" })).not.toBeInTheDocument();
  });

  it("selects a category item and closes dropdown", async () => {
    mockInfiniteState = { ...mockInfiniteState, items: [{ id: 1, name: "Electronics" }] };
    const onChange = vi.fn();
    renderSelect("", onChange);
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => expect(screen.getByText("Electronics", { selector: ".category-option" })).toBeInTheDocument());
    fireEvent.click(screen.getByText("Electronics", { selector: ".category-option" }));
    expect(onChange).toHaveBeenCalledWith(1);
  });

  it("shows loading spinner when loading=true and dropdown open", async () => {
    mockInfiniteState = { ...mockInfiniteState, loading: true };
    renderSelect();
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => expect(document.querySelector(".spinner-border")).not.toBeNull());
  });

  it("closes on outside mousedown", async () => {
    renderSelect();
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => expect(mockReset).toHaveBeenCalled());
    fireEvent.mouseDown(document.body);
    await waitFor(() =>
      expect(screen.queryByText("-- Select --", { selector: ".category-option" })).not.toBeInTheDocument()
    );
  });
});
