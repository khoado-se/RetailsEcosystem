import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";

const mockModalInstance = { show: vi.fn(), hide: vi.fn() };
vi.mock("bootstrap", () => ({
  Modal: { getOrCreateInstance: vi.fn(() => mockModalInstance) },
}));

const { default: ConfirmModal } = await import("@components/ui/ConfirmModal");

describe("ConfirmModal", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders title, message, and confirmLabel", () => {
    render(
      <ConfirmModal
        title="Delete Item"
        message="Are you sure?"
        confirmLabel="Yes, delete"
        onConfirm={vi.fn()}
        onClose={vi.fn()}
      />
    );
    expect(screen.getByText("Delete Item")).toBeInTheDocument();
    expect(screen.getByText("Are you sure?")).toBeInTheDocument();
    expect(screen.getByText("Yes, delete")).toBeInTheDocument();
  });

  it("calls Bootstrap Modal.getOrCreateInstance and show() on mount", () => {
    render(
      <ConfirmModal title="T" message="M" onConfirm={vi.fn()} onClose={vi.fn()} />
    );
    expect(mockModalInstance.show).toHaveBeenCalled();
  });

  it("clicking confirm button calls hide() then onConfirm()", () => {
    const onConfirm = vi.fn();
    render(
      <ConfirmModal title="T" message="M" confirmLabel="OK" onConfirm={onConfirm} onClose={vi.fn()} />
    );
    fireEvent.click(screen.getByText("OK"));
    expect(mockModalInstance.hide).toHaveBeenCalled();
    expect(onConfirm).toHaveBeenCalled();
  });

  it("clicking cancel button calls hide()", () => {
    render(
      <ConfirmModal title="T" message="M" onConfirm={vi.fn()} onClose={vi.fn()} />
    );
    fireEvent.click(screen.getByText("Cancel"));
    expect(mockModalInstance.hide).toHaveBeenCalled();
  });

  it("clicking × button calls hide()", () => {
    render(
      <ConfirmModal title="T" message="M" onConfirm={vi.fn()} onClose={vi.fn()} />
    );
    fireEvent.click(document.querySelector(".btn-close")!);
    expect(mockModalInstance.hide).toHaveBeenCalled();
  });

  it("uses default confirmLabel 'Delete' when not provided", () => {
    render(
      <ConfirmModal title="T" message="M" onConfirm={vi.fn()} onClose={vi.fn()} />
    );
    expect(screen.getByText("Delete")).toBeInTheDocument();
  });
});
