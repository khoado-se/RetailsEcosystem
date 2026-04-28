import apiClient from "../../services/apiClient";

export const login = async (credentials) => {
  const res = await apiClient.post("/auth/login", credentials);
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
