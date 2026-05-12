import apiClient from "../../services/apiClient";

export const getCustomers = async ({ pageNumber, pageSize = 10, search }) => {
  const res = await apiClient.get("/customers", {
    params: { pageNumber, pageSize, search: search || undefined },
  });
  return res.data;
};

export const updateCustomerStatus = (id, isActive) => {
  return apiClient.put(`/customers/${id}/status`, { isActive });
};
