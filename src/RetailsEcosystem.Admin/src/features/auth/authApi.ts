import apiClient from "../../services/apiClient";
import type { AuthResponse, LoginCredentials } from "../../types";

export const login = async (credentials: LoginCredentials): Promise<AuthResponse> => {
  const res = await apiClient.post<AuthResponse>("/auth/login", credentials);
  return res.data;
};

export const refresh = async () => {
  const res = await apiClient.post("/auth/refresh");
  return res.data;
};

export const logout = async () => {
  const res = await apiClient.post("/auth/logout");
  return res.data;
};

export const getMe = async () => {
  const res = await apiClient.get("/auth/me");
  return res.data;
};
