import { describe, it, expect, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { AuthContext } from "@contexts/AuthContext";

vi.mock("@configs/env", () => ({ ENV: { VITE_HOST_URL: "http://api.test" } }));
vi.mock("@services/tokenService", () => ({
  tokenService: { getToken: vi.fn(), getUser: vi.fn(() => null), setToken: vi.fn(), setUser: vi.fn(), clearToken: vi.fn(), clearUser: vi.fn() },
}));

const { default: ProtectedRoute } = await import("@routes/ProtectedRoute");

function makeCtx(overrides = {}) {
  return {
    isAuthenticated: false,
    loading: false,
    user: null,
    login: vi.fn(),
    logout: vi.fn(),
    ...overrides,
  };
}

function renderRoute(ctx, allowedRoles?: string[]) {
  return render(
    <AuthContext.Provider value={ctx}>
      <MemoryRouter initialEntries={["/dashboard"]}>
        <Routes>
          <Route path="/login" element={<span>login-page</span>} />
          <Route element={<ProtectedRoute allowedRoles={allowedRoles} />}>
            <Route path="/dashboard" element={<span>dashboard</span>} />
          </Route>
        </Routes>
      </MemoryRouter>
    </AuthContext.Provider>
  );
}

describe("ProtectedRoute", () => {
  it("shows loading spinner while loading=true", () => {
    renderRoute(makeCtx({ loading: true, isAuthenticated: false }));
    expect(document.querySelector(".spinner-border")).not.toBeNull();
  });

  it("redirects to /login when not authenticated", () => {
    renderRoute(makeCtx({ isAuthenticated: false }));
    expect(screen.getByText("login-page")).toBeInTheDocument();
  });

  it("redirects to /login when authenticated but lacking required role", () => {
    renderRoute(
      makeCtx({ isAuthenticated: true, user: { roles: ["Customer"] } }),
      ["Admin"]
    );
    expect(screen.getByText("login-page")).toBeInTheDocument();
  });

  it("renders Outlet when authenticated with matching role", () => {
    renderRoute(
      makeCtx({ isAuthenticated: true, user: { roles: ["Admin"] } }),
      ["Admin"]
    );
    expect(screen.getByText("dashboard")).toBeInTheDocument();
  });

  it("renders Outlet when authenticated with no allowedRoles restriction", () => {
    renderRoute(makeCtx({ isAuthenticated: true, user: { roles: [] } }));
    expect(screen.getByText("dashboard")).toBeInTheDocument();
  });

  it("renders Outlet when allowedRoles is empty array", () => {
    renderRoute(makeCtx({ isAuthenticated: true, user: { roles: [] } }), []);
    expect(screen.getByText("dashboard")).toBeInTheDocument();
  });
});
