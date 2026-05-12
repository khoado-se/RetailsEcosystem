// Access token stays in memory only — never persisted — to prevent XSS access.
// User metadata (no secrets) is cached in localStorage so the UI can restore
// the authenticated state synchronously on page reload while the async refresh runs.
import type { AuthUser } from "../types";

const USER_KEY = 'admin_user';

let _accessToken: string | null = null;

export const tokenService = {
  getToken: (): string | null => _accessToken,
  setToken: (token: string) => { _accessToken = token; },
  clearToken: () => { _accessToken = null; },

  getUser: (): AuthUser | null => {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? JSON.parse(raw) as AuthUser : null;
    } catch {
      return null;
    }
  },
  setUser: (user: AuthUser) => localStorage.setItem(USER_KEY, JSON.stringify(user)),
  clearUser: () => localStorage.removeItem(USER_KEY),
};
