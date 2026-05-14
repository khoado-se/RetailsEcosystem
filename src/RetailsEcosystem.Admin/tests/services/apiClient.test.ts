import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import MockAdapter from "axios-mock-adapter";

// --- Mocks (must be hoisted before importing apiClient) ---
const mockGetToken = vi.fn();
const mockSetToken = vi.fn();
const mockClearToken = vi.fn();

vi.mock("@services/tokenService", () => ({
  tokenService: {
    getToken: mockGetToken,
    setToken: mockSetToken,
    clearToken: mockClearToken,
  },
}));

vi.mock("@configs/env", () => ({
  ENV: { VITE_HOST_URL: "http://api.test" },
}));

// Import AFTER mocks
const { default: apiClient } = await import("@services/apiClient");

let mock: InstanceType<typeof MockAdapter>;

beforeEach(() => {
  mock = new MockAdapter(apiClient, { onNoMatch: "throwException" });
  vi.clearAllMocks();
  mockGetToken.mockReturnValue(null);
});

afterEach(() => {
  mock.reset();
  mock.restore();
});

// ── Request Interceptor ────────────────────────────────────────────────────────

describe("apiClient — request interceptor", () => {
  it("sends request without Authorization header when no token", async () => {
    mockGetToken.mockReturnValue(null);
    mock.onGet("/ping").reply(200, "ok");

    await apiClient.get("/ping");

    expect(mock.history.get[0].headers?.Authorization).toBeUndefined();
  });

  it("attaches Bearer token when token is present", async () => {
    mockGetToken.mockReturnValue("tok-abc");
    mock.onGet("/ping").reply(200, "ok");

    await apiClient.get("/ping");

    expect(mock.history.get[0].headers?.Authorization).toBe("Bearer tok-abc");
  });
});

// ── Response Interceptor ──────────────────────────────────────────────────────

describe("apiClient — response interceptor (401 handling)", () => {
  it("passes non-401 errors through unchanged", async () => {
    mock.onGet("/data").reply(500, { message: "Server error" });

    await expect(apiClient.get("/data")).rejects.toMatchObject({
      response: { status: 500 },
    });
  });

  it("refreshes token and retries original request on 401", async () => {
    mockGetToken.mockReturnValue("old-token");

    // First call: 401. After refresh the mock allows the retry.
    mock
      .onGet("/protected")
      .replyOnce(401)
      .onGet("/protected")
      .replyOnce(200, { secret: 42 });

    // Intercept the raw axios.post that the interceptor uses for refresh
    const axiosMod = await import("axios");
    const axiosSpy = vi
      .spyOn(axiosMod.default, "post")
      .mockResolvedValueOnce({ data: { accessToken: "new-token" } });

    const res = await apiClient.get("/protected");

    expect(axiosSpy).toHaveBeenCalledOnce();
    expect(mockSetToken).toHaveBeenCalledWith("new-token");
    expect(res.data).toEqual({ secret: 42 });

    axiosSpy.mockRestore();
  });

  it("clears token and redirects to /login when refresh fails on 401", async () => {
    Object.defineProperty(window, "location", {
      value: { href: "" },
      writable: true,
    });

    mock.onGet("/protected").reply(401);

    const axiosMod = await import("axios");
    const axiosSpy = vi
      .spyOn(axiosMod.default, "post")
      .mockRejectedValueOnce(new Error("refresh expired"));

    await expect(apiClient.get("/protected")).rejects.toBeDefined();

    expect(mockClearToken).toHaveBeenCalled();
    expect(window.location.href).toBe("/login");

    axiosSpy.mockRestore();
  });

  it("does not retry a second time when _retry is already set", async () => {
    mock.onGet("/protected").reply(401);

    const axiosMod = await import("axios");
    const axiosSpy = vi
      .spyOn(axiosMod.default, "post")
      .mockRejectedValueOnce(new Error("refresh failed"));

    await expect(apiClient.get("/protected")).rejects.toBeDefined();
    // Only one refresh attempt should be made
    expect(axiosSpy).toHaveBeenCalledOnce();

    axiosSpy.mockRestore();
  });
});
