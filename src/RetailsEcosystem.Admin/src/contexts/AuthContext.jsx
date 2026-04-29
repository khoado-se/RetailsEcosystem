import { createContext, useState, useEffect } from "react";
import axios from "axios";
import { tokenService } from "../services/tokenService";
import { login as loginApi, logout as logoutApi } from "../features/auth/authApi";
import { ENV } from "../configs/env";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
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
        tokenService.setToken(res.data.accessToken);
        setUser(res.data.user);
        setIsAuthenticated(true);
      } catch {
        // No valid refresh token — user must log in.
      } finally {
        setLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = async (credentials) => {
    const data = await loginApi(credentials);
    tokenService.setToken(data.accessToken);
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
      setUser(null);
      setIsAuthenticated(false);
    }
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated, loading, login, logout }}>
      {!loading && children}
    </AuthContext.Provider>
  );
}
