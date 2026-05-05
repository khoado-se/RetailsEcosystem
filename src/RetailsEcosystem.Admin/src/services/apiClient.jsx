import axios from "axios";
import { ENV } from "../configs/env";
import { tokenService } from "./tokenService";

const apiClient = axios.create({
  baseURL: ENV.VITE_HOST_URL,
  timeout: 5000,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true, // Crucial for sending/receiving httpOnly cookies
});

// Request Interceptor: Attach Access Token
apiClient.interceptors.request.use(
  (config) => {
    const token = tokenService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Shared promise for in-flight refresh — prevents concurrent 401s from each
// calling /auth/refresh independently (race condition: first call rotates the
// refresh token, making every subsequent call fail and logging the user out).
let refreshPromise = null;

// Response Interceptor: Handle 401s and Refresh Token
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // If error is 401 and we haven't retried yet
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // Deduplicate: all concurrent 401s share one refresh call.
        // refreshPromise is cleared in .finally() so the next genuine
        // expiry (after a successful retry) starts a fresh refresh.
        if (!refreshPromise) {
          refreshPromise = axios
            .post(`${ENV.VITE_HOST_URL}/auth/refresh`, {}, { withCredentials: true })
            .finally(() => { refreshPromise = null; });
        }

        const res = await refreshPromise;
        const newAccessToken = res.data.accessToken;
        tokenService.setToken(newAccessToken);

        // Retry the original request with the new token
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return apiClient(originalRequest);
      } catch {
        // Refresh failed (token expired / revoked) — clear state and redirect
        tokenService.clearToken();
        window.location.href = "/login";
        return Promise.reject(error);
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;