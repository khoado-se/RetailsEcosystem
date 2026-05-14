import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import CategoryTable from "@features/category/CategoryTable";

const CATS = [
  { id: 1, name: "Electronics" },
  { id: 2, name: "Books" },
];

describe("CategoryTable", () => {
  it("renders No categories found when list is empty", () => {
    render(<CategoryTable categories={[]} onEdit={vi.fn()} onDelete={vi.fn()} />);
    expect(screen.getByText("No categories found.")).toBeInTheDocument();
  });

  it("renders one row per category", () => {
    render(<CategoryTable categories={CATS} onEdit={vi.fn()} onDelete={vi.fn()} />);
    expect(screen.getByText("Electronics")).toBeInTheDocument();
    expect(screen.getByText("Books")).toBeInTheDocument();
  });

  it("clicking Edit calls onEdit with full category object", () => {
    const onEdit = vi.fn();
    render(<CategoryTable categories={CATS} onEdit={onEdit} onDelete={vi.fn()} />);
    const editBtns = screen.getAllByText("Edit");
    fireEvent.click(editBtns[0]);
    expect(onEdit).toHaveBeenCalledWith(CATS[0]);
  });

  it("clicking Delete calls onDelete with category id", () => {
    const onDelete = vi.fn();
    render(<CategoryTable categories={CATS} onEdit={vi.fn()} onDelete={onDelete} />);
    const deleteBtns = screen.getAllByText("Delete");
    fireEvent.click(deleteBtns[1]);
    expect(onDelete).toHaveBeenCalledWith(2);
  });
});
