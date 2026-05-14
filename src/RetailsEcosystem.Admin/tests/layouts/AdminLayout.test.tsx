import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthContext } from "@contexts/AuthContext";

// Mock Bootstrap Collapse used by Sidebar
const mockShow = vi.fn();
const mockHide = vi.fn();
vi.mock("bootstrap", () => ({
  Collapse: {
    getOrCreateInstance: vi.fn(() => ({ show: mockShow, hide: mockHide })),
  },
}));

// Mock react-hot-toast Toaster
vi.mock("react-hot-toast", () => ({
  Toaster: () => <div data-testid="toaster" />,
}));

// Mock the DashboardPage CSS import
vi.mock("../../src/pages/dashboard/DashboardPage.css", () => ({}));

import AdminLayout from "../../src/layouts/AdminLayout";

function renderLayout() {
  const ctx = {
    isAuthenticated: true,
    loading: false,
    user: { fullName: "Test Admin", roles: ["Admin"] },
    login: vi.fn(),
    logout: vi.fn(),
  };
  return render(
    <AuthContext.Provider value={ctx as any}>
      <MemoryRouter>
        <AdminLayout />
      </MemoryRouter>
    </AuthContext.Provider>
  );
}

describe("AdminLayout", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders Header with brand logo", () => {
    renderLayout();
    expect(screen.getByAltText("RetailsEcosystem")).toBeInTheDocument();
  });

  it("renders Sidebar navigation links", () => {
    renderLayout();
    expect(screen.getByText("Dashboard")).toBeInTheDocument();
    expect(screen.getByText("Products")).toBeInTheDocument();
    expect(screen.getByText("Categories")).toBeInTheDocument();
  });

  it("renders Main content area", () => {
    renderLayout();
    expect(screen.getByRole("main")).toBeInTheDocument();
  });

  it("renders Toaster component", () => {
    renderLayout();
    expect(screen.getByTestId("toaster")).toBeInTheDocument();
  });

  it("toggles sidebar open/close when hamburger button is clicked", () => {
    renderLayout();
    const toggleBtn = screen.getByLabelText("Toggle navigation");

    // Click to open
    fireEvent.click(toggleBtn);
    expect(mockShow).toHaveBeenCalled();

    // Click again to close
    fireEvent.click(toggleBtn);
    expect(mockHide).toHaveBeenCalled();
  });
});
