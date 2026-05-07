import { describe, it, expect, beforeEach, vi } from "vitest";

// Reset module registry before each test so the in-memory _accessToken is fresh
beforeEach(() => {
  vi.resetModules();
  localStorage.clear();
});

describe("tokenService — access token (in-memory)", () => {
  it("getToken returns null before any token is set", async () => {
    const { tokenService } = await import("@services/tokenService");
    expect(tokenService.getToken()).toBeNull();
  });

  it("setToken then getToken returns the stored token", async () => {
    const { tokenService } = await import("@services/tokenService");
    tokenService.setToken("abc-123");
    expect(tokenService.getToken()).toBe("abc-123");
  });

  it("clearToken resets token to null", async () => {
    const { tokenService } = await import("@services/tokenService");
    tokenService.setToken("abc-123");
    tokenService.clearToken();
    expect(tokenService.getToken()).toBeNull();
  });
});

describe("tokenService — user cache (localStorage)", () => {
  it("setUser persists JSON and getUser parses it back", async () => {
    const { tokenService } = await import("@services/tokenService");
    const user = { id: "u1", email: "test@example.com" };
    tokenService.setUser(user);
    expect(tokenService.getUser()).toEqual(user);
  });

  it("clearUser removes user and getUser returns null", async () => {
    const { tokenService } = await import("@services/tokenService");
    tokenService.setUser({ id: "u1" });
    tokenService.clearUser();
    expect(tokenService.getUser()).toBeNull();
  });

  it("getUser returns null when localStorage contains corrupt JSON", async () => {
    localStorage.setItem("admin_user", "{bad json");
    const { tokenService } = await import("@services/tokenService");
    expect(tokenService.getUser()).toBeNull();
  });
});
