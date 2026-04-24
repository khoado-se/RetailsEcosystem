import apiClient from "../../app/apiClient";

// POST
export const updateProduct = (productId, data) => {
  return apiClient.post(`/productImages/upload?productId=${productId}`, data);
};
