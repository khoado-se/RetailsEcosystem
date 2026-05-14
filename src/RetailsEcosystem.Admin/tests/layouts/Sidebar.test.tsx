import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";

// Mock Bootstrap Collapse
const mockShow = vi.fn();
const mockHide = vi.fn();
vi.mock("bootstrap", () => ({
  Collapse: {
    getOrCreateInstance: vi.fn(() => ({ show: mockShow, hide: mockHide })),
  },
}));

import Sidebar from "../../src/layouts/Sidebar";

const onClose = vi.fn();

function renderSidebar(isOpen = false, initialPath = "/") {
  return render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Sidebar isOpen={isOpen} onClose={onClose} />
    </MemoryRouter>
  );
}

describe("Sidebar", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders all navigation links", () => {
    renderSidebar();
    expect(screen.getByText("Dashboard")).toBeInTheDocument();
    expect(screen.getByText("Orders")).toBeInTheDocument();
    expect(screen.getByText("Products")).toBeInTheDocument();
    expect(screen.getByText("Categories")).toBeInTheDocument();
    expect(screen.getByText("Customers")).toBeInTheDocument();
  });

  it("has correct link destinations", () => {
    renderSidebar();
    expect(screen.getByText("Dashboard").closest("a")).toHaveAttribute("href", "/");
    expect(screen.getByText("Orders").closest("a")).toHaveAttribute("href", "/orders");
    expect(screen.getByText("Products").closest("a")).toHaveAttribute("href", "/products");
    expect(screen.getByText("Categories").closest("a")).toHaveAttribute("href", "/categories");
    expect(screen.getByText("Customers").closest("a")).toHaveAttribute("href", "/customers");
  });

  it("calls Collapse.show() when isOpen is true", () => {
    renderSidebar(true);
    expect(mockShow).toHaveBeenCalled();
  });

  it("calls Collapse.hide() when isOpen is false", () => {
    renderSidebar(false);
    expect(mockHide).toHaveBeenCalled();
  });

  it("calls onClose on mount (location effect)", () => {
    renderSidebar();
    // The useEffect on location.pathname fires on mount
    expect(onClose).toHaveBeenCalled();
  });

  it("renders nav element with sidebar ID", () => {
    renderSidebar();
    const nav = screen.getByRole("navigation");
    expect(nav).toHaveAttribute("id", "sidebarMenu");
  });
});
