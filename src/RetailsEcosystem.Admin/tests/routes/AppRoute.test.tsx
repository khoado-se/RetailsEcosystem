import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthContext } from "@contexts/AuthContext";

// Mock Bootstrap Collapse used by Sidebar
vi.mock("bootstrap", () => ({
  Collapse: {
    getOrCreateInstance: vi.fn(() => ({ show: vi.fn(), hide: vi.fn() })),
  },
}));

// Mock heavy page components to avoid pulling in their dependencies
vi.mock("../../src/pages/login/LoginPage", () => ({
  default: () => <div data-testid="login-page">LoginPage</div>,
}));
vi.mock("../../src/pages/dashboard/DashboardPage", () => ({
  default: () => <div data-testid="dashboard-page">DashboardPage</div>,
}));
vi.mock("../../src/pages/order/OrdersPage", () => ({
  default: () => <div data-testid="orders-page">OrdersPage</div>,
}));
vi.mock("../../src/pages/products/ProductsPage", () => ({
  default: () => <div data-testid="products-page">ProductsPage</div>,
}));
vi.mock("../../src/pages/categories/CategoriesPage", () => ({
  default: () => <div data-testid="categories-page">CategoriesPage</div>,
}));
vi.mock("../../src/pages/customers/CustomerListPage", () => ({
  default: () => <div data-testid="customers-page">CustomerListPage</div>,
}));
vi.mock("../../src/pages/error/notfound/NotFoundPage", () => ({
  default: () => <div data-testid="notfound-page">NotFoundPage</div>,
}));

// Mock DashboardPage CSS
vi.mock("../../src/pages/dashboard/DashboardPage.css", () => ({}));
// Mock App.css
vi.mock("../../src/App.css", () => ({}));
// Mock react-hot-toast
vi.mock("react-hot-toast", () => ({
  Toaster: () => <div data-testid="toaster" />,
}));

import AppRoute from "../../src/routes/AppRoute";

function renderRoute(
  path: string,
  { isAuthenticated = false, user = null as any, loading = false } = {}
) {
  const ctx = {
    isAuthenticated,
    loading,
    user,
    login: vi.fn(),
    logout: vi.fn(),
  };
  return render(
    <AuthContext.Provider value={ctx}>
      <MemoryRouter initialEntries={[path]}>
        <AppRoute />
      </MemoryRouter>
    </AuthContext.Provider>
  );
}

describe("AppRoute", () => {
  it("renders LoginPage on /login", () => {
    renderRoute("/login");
    expect(screen.getByTestId("login-page")).toBeInTheDocument();
  });

  it("redirects to /login when not authenticated and visiting /", () => {
    renderRoute("/", { isAuthenticated: false });
    // ProtectedRoute should redirect to /login, so LoginPage shows
    expect(screen.getByTestId("login-page")).toBeInTheDocument();
  });

  it("renders DashboardPage on / when authenticated with Admin role", () => {
    renderRoute("/", {
      isAuthenticated: true,
      user: { fullName: "Admin", roles: ["Admin"] },
    });
    expect(screen.getByTestId("dashboard-page")).toBeInTheDocument();
  });

  it("renders ProductsPage on /products when authenticated", () => {
    renderRoute("/products", {
      isAuthenticated: true,
      user: { fullName: "Admin", roles: ["Admin"] },
    });
    expect(screen.getByTestId("products-page")).toBeInTheDocument();
  });

  it("renders CategoriesPage on /categories when authenticated", () => {
    renderRoute("/categories", {
      isAuthenticated: true,
      user: { fullName: "Admin", roles: ["Admin"] },
    });
    expect(screen.getByTestId("categories-page")).toBeInTheDocument();
  });

  it("renders OrdersPage on /orders when authenticated", () => {
    renderRoute("/orders", {
      isAuthenticated: true,
      user: { fullName: "Admin", roles: ["Admin"] },
    });
    expect(screen.getByTestId("orders-page")).toBeInTheDocument();
  });

  it("renders CustomerListPage on /customers when authenticated", () => {
    renderRoute("/customers", {
      isAuthenticated: true,
      user: { fullName: "Admin", roles: ["Admin"] },
    });
    expect(screen.getByTestId("customers-page")).toBeInTheDocument();
  });

  it("renders NotFoundPage on unknown route when authenticated", () => {
    renderRoute("/nonexistent", {
      isAuthenticated: true,
      user: { fullName: "Admin", roles: ["Admin"] },
    });
    expect(screen.getByTestId("notfound-page")).toBeInTheDocument();
  });
});
