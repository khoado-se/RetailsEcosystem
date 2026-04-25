import axios from "axios";

import { ENV } from "../configs/env";

const apiClient = axios.create({
  baseURL: ENV.VITE_HOST_URL,
  timeout: 5000,
  headers: {
    "Content-Type": "application/json",
  },
});

export default apiClient;