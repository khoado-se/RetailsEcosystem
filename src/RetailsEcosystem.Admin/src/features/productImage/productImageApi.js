import apiClient from "../../services/apiClient";

// POST
export const uploadProductImage = (productId, data) => {
  return apiClient.post(`/productImages/upload?productId=${productId}`, data);
};
