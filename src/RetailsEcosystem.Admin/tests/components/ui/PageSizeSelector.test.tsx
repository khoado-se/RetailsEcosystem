import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import PageSizeSelector from "@components/ui/PageSizeSelector";

describe("PageSizeSelector", () => {
  it("renders select with options 10, 20, 50", () => {
    render(<PageSizeSelector pageSize={10} onPageSizeChange={vi.fn()} />);
    const select = screen.getByRole("combobox") as HTMLSelectElement;
    const values = Array.from(select.options).map((o) => Number(o.value));
    expect(values).toEqual([10, 20, 50]);
  });

  it("shows current pageSize as selected", () => {
    render(<PageSizeSelector pageSize={20} onPageSizeChange={vi.fn()} />);
    const select = screen.getByRole("combobox") as HTMLSelectElement;
    expect(Number(select.value)).toBe(20);
  });

  it("calls onPageSizeChange with Number when selection changes", () => {
    const onChange = vi.fn();
    render(<PageSizeSelector pageSize={10} onPageSizeChange={onChange} />);
    fireEvent.change(screen.getByRole("combobox"), { target: { value: "50" } });
    expect(onChange).toHaveBeenCalledWith(50);
  });
});
