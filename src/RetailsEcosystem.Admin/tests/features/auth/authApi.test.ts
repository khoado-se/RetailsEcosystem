import { describe, it, expect, vi, beforeEach } from "vitest";

const mockPost = vi.fn();
const mockGet = vi.fn();

vi.mock("@services/apiClient", () => ({ default: { post: mockPost, get: mockGet } }));

const { login, refresh, logout, getMe } = await import("@features/auth/authApi");

describe("authApi", () => {
  beforeEach(() => vi.clearAllMocks());

  it("login posts credentials and returns data", async () => {
    mockPost.mockResolvedValue({ data: { accessToken: "tok", user: { id: "u1" } } });
    const result = await login({ email: "a@b.com", password: "pw" });
    expect(mockPost).toHaveBeenCalledWith("/auth/login", { email: "a@b.com", password: "pw" });
    expect(result).toEqual({ accessToken: "tok", user: { id: "u1" } });
  });

  it("refresh posts to /auth/refresh and returns data", async () => {
    mockPost.mockResolvedValue({ data: { accessToken: "new-tok" } });
    const result = await refresh();
    expect(mockPost).toHaveBeenCalledWith("/auth/refresh");
    expect(result).toEqual({ accessToken: "new-tok" });
  });

  it("logout posts to /auth/logout and returns data", async () => {
    mockPost.mockResolvedValue({ data: null });
    const result = await logout();
    expect(mockPost).toHaveBeenCalledWith("/auth/logout");
    expect(result).toBeNull();
  });

  it("getMe GETs /auth/me and returns data", async () => {
    mockGet.mockResolvedValue({ data: { id: "u1", email: "a@b.com" } });
    const result = await getMe();
    expect(mockGet).toHaveBeenCalledWith("/auth/me");
    expect(result).toEqual({ id: "u1", email: "a@b.com" });
  });
});
