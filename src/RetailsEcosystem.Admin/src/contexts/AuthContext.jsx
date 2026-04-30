import { createContext, useState, useEffect } from "react";
import axios from "axios";
import { tokenService } from "../services/tokenService";
import { login as loginApi, logout as logoutApi } from "../features/auth/authApi";
import { ENV } from "../configs/env";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
  // Synchronously restore from localStorage — prevents redirect-to-login flash on reload.
  const [user, setUser] = useState(() => tokenService.getUser());
  const [isAuthenticated, setIsAuthenticated] = useState(() => !!tokenService.getUser());
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      try {
        // Use raw axios to bypass the apiClient response interceptor (avoids retry loop).
        // The httpOnly refresh-token cookie is sent automatically via withCredentials.
        const res = await axios.post(
          `${ENV.VITE_HOST_URL}/auth/refresh`,
          {},
          { withCredentials: true }
        );
        const { accessToken, user: freshUser } = res.data ?? {};
        if (!accessToken || !freshUser) throw new Error("Invalid refresh response");

        tokenService.setToken(accessToken);
        tokenService.setUser(freshUser);
        setUser(freshUser);
        setIsAuthenticated(true);
      } catch {
        // Refresh token absent, expired, or response invalid — clear stale cache.
        tokenService.clearToken();
        tokenService.clearUser();
        setUser(null);
        setIsAuthenticated(false);
      } finally {
        setLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = async (credentials) => {
    const data = await loginApi(credentials);
    tokenService.setToken(data.accessToken);
    tokenService.setUser(data.user);
    setUser(data.user);
    setIsAuthenticated(true);
  };

  const logout = async () => {
    try {
      await logoutApi();
    } catch (e) {
      console.error("Logout error", e);
    } finally {
      tokenService.clearToken();
      tokenService.clearUser();
      setUser(null);
      setIsAuthenticated(false);
    }
  };

  // Always render children — loading state is handled by ProtectedRoute.
  // Hiding the entire tree here would also hide BrowserRouter and the /login route.
  return (
    <AuthContext.Provider value={{ user, isAuthenticated, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
