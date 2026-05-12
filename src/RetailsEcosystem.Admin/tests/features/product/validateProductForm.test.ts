import { describe, it, expect } from "vitest";
import { validateProductForm } from "@features/product/validateProductForm";

describe("validateProductForm", () => {
  it("returns empty array when all fields are valid", () => {
    const result = validateProductForm({ name: "Phone", price: 99, categoryId: 3 });
    expect(result).toEqual([]);
  });

  it("returns error when name is empty string", () => {
    const result = validateProductForm({ name: "", price: 99, categoryId: 3 });
    expect(result).toContain("Product name is required.");
  });

  it("returns error when name is whitespace only", () => {
    const result = validateProductForm({ name: "   ", price: 99, categoryId: 3 });
    expect(result).toContain("Product name is required.");
  });

  it("returns error when price is zero", () => {
    const result = validateProductForm({ name: "X", price: 0, categoryId: 3 });
    expect(result).toContain("Price must be greater than 0.");
  });

  it("returns error when price is negative", () => {
    const result = validateProductForm({ name: "X", price: -5, categoryId: 3 });
    expect(result).toContain("Price must be greater than 0.");
  });

  it("returns error when categoryId is null", () => {
    const result = validateProductForm({ name: "X", price: 10, categoryId: null });
    expect(result).toContain("Please select a category.");
  });

  it("returns all three errors when all fields are invalid", () => {
    const result = validateProductForm({ name: "", price: 0, categoryId: null });
    expect(result).toHaveLength(3);
  });
});
