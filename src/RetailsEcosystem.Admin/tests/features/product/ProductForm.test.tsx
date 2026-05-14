import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";

// Mock CategorySelect — has IntersectionObserver complexity
vi.mock("@features/category/CategorySelect", () => ({
  default: ({ value, onChange }: { value: string; onChange: (id: string) => void }) => (
    <select
      data-testid="cat-select"
      value={value}
      onChange={(e) => onChange(e.target.value)}
    >
      <option value="">-- Select --</option>
      <option value="1">Electronics</option>
    </select>
  ),
}));

const { default: ProductForm } = await import("@features/product/ProductForm");

function makeForm(overrides = {}) {
  return {
    name: "Phone",
    price: "999",
    categoryId: "1",
    categoryName: "Electronics",
    description: "A phone",
    isFeatured: false,
    ...overrides,
  };
}

describe("ProductForm", () => {
  it("renders name, price, description, and isFeatured inputs", () => {
    render(<ProductForm form={makeForm()} handleChange={vi.fn()} imageSection={null} />);
    expect(screen.getByDisplayValue("Phone")).toBeInTheDocument();
    expect(screen.getByDisplayValue("999")).toBeInTheDocument();
    expect(screen.getByDisplayValue("A phone")).toBeInTheDocument();
    expect(screen.getByRole("checkbox")).toBeInTheDocument();
  });

  it("calls handleChange when name input changes", () => {
    const handleChange = vi.fn();
    render(<ProductForm form={makeForm()} handleChange={handleChange} imageSection={null} />);
    fireEvent.change(screen.getByDisplayValue("Phone"), { target: { name: "name", value: "Laptop" } });
    expect(handleChange).toHaveBeenCalled();
  });

  it("calls handleChange with synthetic event when category selected via CategorySelect", () => {
    const handleChange = vi.fn();
    render(<ProductForm form={makeForm({ categoryId: "" })} handleChange={handleChange} imageSection={null} />);
    fireEvent.change(screen.getByTestId("cat-select"), { target: { value: "1" } });
    expect(handleChange).toHaveBeenCalledWith(
      expect.objectContaining({ target: expect.objectContaining({ name: "categoryId", value: "1" }) })
    );
  });

  it("renders imageSection slot when provided", () => {
    render(
      <ProductForm
        form={makeForm()}
        handleChange={vi.fn()}
        imageSection={<div data-testid="img-section">images</div>}
      />
    );
    expect(screen.getByTestId("img-section")).toBeInTheDocument();
  });

  it("checkbox is checked when isFeatured=true", () => {
    render(<ProductForm form={makeForm({ isFeatured: true })} handleChange={vi.fn()} imageSection={null} />);
    expect(screen.getByRole("checkbox")).toBeChecked();
  });
});
