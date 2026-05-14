import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, act, waitFor } from "@testing-library/react";
import { useContext } from "react";

// --- Mocks ---
const mockSetToken = vi.fn();
const mockSetUser = vi.fn();
const mockClearToken = vi.fn();
const mockClearUser = vi.fn();
const mockGetUser = vi.fn(() => null);

vi.mock("@services/tokenService", () => ({
  tokenService: {
    getToken: vi.fn(() => null),
    setToken: mockSetToken,
    clearToken: mockClearToken,
    getUser: mockGetUser,
    setUser: mockSetUser,
    clearUser: mockClearUser,
  },
}));

vi.mock("@configs/env", () => ({
  ENV: { VITE_HOST_URL: "http://api.test" },
}));

const mockLoginApi = vi.fn();
const mockLogoutApi = vi.fn();

vi.mock("@features/auth/authApi", () => ({
  login: mockLoginApi,
  logout: mockLogoutApi,
}));

const mockAxiosPost = vi.fn();
vi.mock("axios", async (importOriginal) => {
  const actual = await importOriginal<typeof import("axios")>();
  return { ...actual, default: { ...actual.default, post: mockAxiosPost } };
});

// Import AFTER mocks
const { AuthContext, AuthProvider } = await import("@contexts/AuthContext");

// Helper: consumer component
function Consumer() {
  const { isAuthenticated, loading, user } = useContext(AuthContext);
  if (loading) return <span>loading</span>;
  return (
    <span>
      {isAuthenticated ? "auth" : "unauth"},{user ? user.email : "no-user"}
    </span>
  );
}

function Wrapper({ children = <Consumer /> } = {}) {
  return <AuthProvider>{children}</AuthProvider>;
}

describe("AuthProvider — initial refresh on mount", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows loading spinner initially then resolves authenticated", async () => {
    mockAxiosPost.mockResolvedValueOnce({
      data: { accessToken: "tok", user: { id: "u1", email: "a@b.com" } },
    });

    const { getByText } = render(<Wrapper />);
    expect(getByText("loading")).toBeInTheDocument();

    await waitFor(() => expect(getByText(/auth,a@b\.com/)).toBeInTheDocument());
    expect(mockSetToken).toHaveBeenCalledWith("tok");
    expect(mockSetUser).toHaveBeenCalledWith({ id: "u1", email: "a@b.com" });
  });

  it("sets isAuthenticated=false when refresh returns no accessToken", async () => {
    mockAxiosPost.mockResolvedValueOnce({ data: { user: { id: "u1" } } });

    render(<Wrapper />);
    await waitFor(() => expect(screen.getByText(/unauth/)).toBeInTheDocument());

    expect(mockClearToken).toHaveBeenCalled();
    expect(mockClearUser).toHaveBeenCalled();
  });

  it("sets isAuthenticated=false when refresh throws", async () => {
    mockAxiosPost.mockRejectedValueOnce(new Error("no refresh cookie"));

    render(<Wrapper />);
    await waitFor(() => expect(screen.getByText(/unauth/)).toBeInTheDocument());

    expect(mockClearToken).toHaveBeenCalled();
    expect(mockClearUser).toHaveBeenCalled();
  });
});

describe("AuthProvider — login()", () => {
  beforeEach(() => vi.clearAllMocks());

  it("calls loginApi and sets token/user on success", async () => {
    mockAxiosPost.mockRejectedValueOnce(new Error("no cookie"));
    mockLoginApi.mockResolvedValueOnce({
      accessToken: "login-tok",
      user: { id: "u2", email: "login@test.com" },
    });

    function LoginConsumer() {
      const ctx = useContext(AuthContext);
      if (ctx.loading) return <span>loading</span>;
      return (
        <button onClick={() => ctx.login({ email: "login@test.com", password: "pw" })}>
          {ctx.isAuthenticated ? "authed" : "login"}
        </button>
      );
    }

    render(<AuthProvider><LoginConsumer /></AuthProvider>);
    await waitFor(() => expect(screen.getByText("login")).toBeInTheDocument());

    await act(async () => {
      screen.getByText("login").click();
    });

    expect(mockSetToken).toHaveBeenCalledWith("login-tok");
    expect(mockSetUser).toHaveBeenCalledWith({ id: "u2", email: "login@test.com" });
  });
});

describe("AuthProvider — logout()", () => {
  beforeEach(() => vi.clearAllMocks());

  it("clears token/user on successful logout", async () => {
    mockAxiosPost.mockRejectedValueOnce(new Error("no cookie"));
    mockLogoutApi.mockResolvedValueOnce(undefined);

    function LogoutConsumer() {
      const ctx = useContext(AuthContext);
      if (ctx.loading) return <span>loading</span>;
      return <button onClick={() => ctx.logout()}>logout</button>;
    }

    render(<AuthProvider><LogoutConsumer /></AuthProvider>);
    await waitFor(() => expect(screen.getByText("logout")).toBeInTheDocument());

    await act(async () => { screen.getByText("logout").click(); });

    expect(mockClearToken).toHaveBeenCalled();
    expect(mockClearUser).toHaveBeenCalled();
  });

  it("still clears state even when logoutApi throws", async () => {
    mockAxiosPost.mockRejectedValueOnce(new Error("no cookie"));
    mockLogoutApi.mockRejectedValueOnce(new Error("network error"));

    function LogoutConsumer() {
      const ctx = useContext(AuthContext);
      if (ctx.loading) return <span>loading</span>;
      return <button onClick={() => ctx.logout()}>logout</button>;
    }

    render(<AuthProvider><LogoutConsumer /></AuthProvider>);
    await waitFor(() => expect(screen.getByText("logout")).toBeInTheDocument());

    await act(async () => { screen.getByText("logout").click(); });

    expect(mockClearToken).toHaveBeenCalled();
    expect(mockClearUser).toHaveBeenCalled();
  });
});
