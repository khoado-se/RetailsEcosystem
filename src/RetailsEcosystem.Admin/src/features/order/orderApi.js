import apiClient from "../../services/apiClient";

export const getOrders = ({ pageNumber = 1, pageSize = 10, status } = {}) =>
  apiClient.get("/orders", {
    params: { pageNumber, pageSize, ...(status != null ? { status } : {}) },
  });

export const getOrderById = (id) => apiClient.get(`/orders/${id}`);

export const updateOrderStatus = (id, status) =>
  apiClient.put(`/orders/${id}/status`, { status });

export const getOrderStats = () => apiClient.get("/orders/stats");
