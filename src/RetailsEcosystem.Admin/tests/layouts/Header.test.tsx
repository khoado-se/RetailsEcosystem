import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthContext } from "@contexts/AuthContext";
import Header from "../../src/layouts/Header";

const mockLogout = vi.fn();

function renderHeader(
  user: { fullName: string; roles: string[] } | null = { fullName: "Admin User", roles: ["Admin"] },
  onToggleSidebar = vi.fn()
) {
  const ctx = {
    isAuthenticated: !!user,
    loading: false,
    user,
    login: vi.fn(),
    logout: mockLogout,
  };
  return render(
    <AuthContext.Provider value={ctx as any}>
      <MemoryRouter>
        <Header onToggleSidebar={onToggleSidebar} />
      </MemoryRouter>
    </AuthContext.Provider>
  );
}

describe("Header", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders the brand logo", () => {
    renderHeader();
    const logo = screen.getByAltText("RetailsEcosystem");
    expect(logo).toBeInTheDocument();
    expect(logo.tagName).toBe("IMG");
  });

  it("renders Sign out button", () => {
    renderHeader();
    expect(screen.getByText("Sign out")).toBeInTheDocument();
  });

  it("displays user avatar initial and full name when user is present", () => {
    renderHeader({ fullName: "John Doe", roles: ["Admin"] });
    expect(screen.getByText("J")).toBeInTheDocument();
    expect(screen.getByText("John Doe")).toBeInTheDocument();
  });

  it("displays '?' as avatar initial when fullName is empty", () => {
    renderHeader({ fullName: "", roles: ["Admin"] });
    expect(screen.getByText("?")).toBeInTheDocument();
  });

  it("does not display user info when user is null", () => {
    renderHeader(null);
    expect(screen.queryByText("Admin User")).not.toBeInTheDocument();
  });

  it("calls logout when Sign out is clicked", async () => {
    mockLogout.mockResolvedValueOnce(undefined);
    renderHeader();
    fireEvent.click(screen.getByText("Sign out"));
    expect(mockLogout).toHaveBeenCalledTimes(1);
  });

  it("calls onToggleSidebar when mobile menu button is clicked", () => {
    const onToggle = vi.fn();
    renderHeader({ fullName: "Admin", roles: ["Admin"] }, onToggle);
    const toggleBtn = screen.getByLabelText("Toggle navigation");
    fireEvent.click(toggleBtn);
    expect(onToggle).toHaveBeenCalledTimes(1);
  });
});
