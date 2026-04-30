// Access token stays in memory only — never persisted — to prevent XSS access.
// User metadata (no secrets) is cached in localStorage so the UI can restore
// the authenticated state synchronously on page reload while the async refresh runs.
const USER_KEY = 'admin_user';

let _accessToken = null;

export const tokenService = {
  getToken: () => _accessToken,
  setToken: (token) => { _accessToken = token; },
  clearToken: () => { _accessToken = null; },

  getUser: () => {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  },
  setUser: (user) => localStorage.setItem(USER_KEY, JSON.stringify(user)),
  clearUser: () => localStorage.removeItem(USER_KEY),
};
