import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { AuthContext } from "@contexts/AuthContext";

vi.mock("@configs/env", () => ({ ENV: { VITE_HOST_URL: "http://api.test" } }));
vi.mock("@services/tokenService", () => ({
  tokenService: { getToken: vi.fn(), getUser: vi.fn(() => null), setToken: vi.fn(), setUser: vi.fn(), clearToken: vi.fn(), clearUser: vi.fn() },
}));

const mockNavigate = vi.fn();

vi.mock("react-router-dom", async (importOriginal) => {
  const actual = await importOriginal<typeof import("react-router-dom")>();
  return { ...actual, useNavigate: () => mockNavigate };
});

// Import AFTER all mocks
const { default: LoginPage } = await import("@pages/login/LoginPage");

const mockLogin = vi.fn();

function renderLogin(loginFn = mockLogin) {
  const ctx = {
    isAuthenticated: false, loading: false, user: null,
    login: loginFn, logout: vi.fn(),
  };
  return render(
    <AuthContext.Provider value={ctx}>
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>
    </AuthContext.Provider>
  );
}

describe("LoginPage — rendering", () => {
  it("renders email and password inputs", () => {
    renderLogin();
    expect(screen.getByPlaceholderText(/example\.com/i)).toBeInTheDocument();
    expect(screen.getByPlaceholderText(/password/i)).toBeInTheDocument();
  });

  it("renders Sign In button", () => {
    renderLogin();
    expect(screen.getByRole("button", { name: /sign in/i })).toBeInTheDocument();
  });
});

describe("LoginPage — form interactions", () => {
  beforeEach(() => vi.clearAllMocks());

  it("calls login() with email and password on submit", async () => {
    mockLogin.mockResolvedValueOnce(undefined);
    renderLogin();
    fireEvent.change(screen.getByPlaceholderText(/example\.com/i), { target: { value: "a@b.com" } });
    fireEvent.change(screen.getByPlaceholderText(/password/i), { target: { value: "pass123" } });
    fireEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await waitFor(() => expect(mockLogin).toHaveBeenCalledWith({ email: "a@b.com", password: "pass123" }));
    expect(mockNavigate).toHaveBeenCalled();
  });

  it("shows error message when login throws with response.data.detail", async () => {
    mockLogin.mockRejectedValueOnce({ response: { data: { detail: "Invalid credentials" } } });
    renderLogin();
    fireEvent.change(screen.getByPlaceholderText(/example\.com/i), { target: { value: "x@y.com" } });
    fireEvent.change(screen.getByPlaceholderText(/password/i), { target: { value: "wrong" } });
    fireEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await waitFor(() => expect(screen.getByText("Invalid credentials")).toBeInTheDocument());
  });

  it("shows fallback error message when login throws with no detail", async () => {
    mockLogin.mockRejectedValueOnce({ response: { data: {} } });
    renderLogin();
    fireEvent.change(screen.getByPlaceholderText(/example\.com/i), { target: { value: "x@y.com" } });
    fireEvent.change(screen.getByPlaceholderText(/password/i), { target: { value: "wrong" } });
    fireEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await waitFor(() => expect(screen.getByText(/Failed to login/i)).toBeInTheDocument());
  });

  it("toggles password visibility on show/hide button click", () => {
    renderLogin();
    const pwInput = screen.getByPlaceholderText(/password/i) as HTMLInputElement;
    expect(pwInput.type).toBe("password");
    const toggleBtn = pwInput.parentElement?.querySelector("button");
    if (toggleBtn) fireEvent.click(toggleBtn);
    expect(pwInput.type).toBe("text");
  });

  it("shows errors from response.data.errors array", async () => {
    mockLogin.mockRejectedValueOnce({ response: { data: { errors: ["Email invalid"] } } });
    renderLogin();
    fireEvent.change(screen.getByPlaceholderText(/example\.com/i), { target: { value: "x" } });
    fireEvent.change(screen.getByPlaceholderText(/password/i), { target: { value: "p" } });
    fireEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await waitFor(() => expect(screen.getByText("Email invalid")).toBeInTheDocument());
  });
});
