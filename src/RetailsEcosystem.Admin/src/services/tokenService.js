// Access token is stored only in memory — never persisted to localStorage/sessionStorage.
// This eliminates XSS access to the token. Session survival across page reloads is
// handled by the httpOnly refresh-token cookie (see AuthContext initAuth).
let _accessToken = null;

export const tokenService = {
  getToken: () => _accessToken,
  setToken: (token) => { _accessToken = token; },
  clearToken: () => { _accessToken = null; },
};
