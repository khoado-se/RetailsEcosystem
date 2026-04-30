import apiClient from "../../services/apiClient";

export const uploadProductImage = (productId, formData) =>
  apiClient.post(`/productImages/upload?productId=${productId}`, formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });

export const getProductImages = (productId) =>
  apiClient.get(`/productImages/by-product/${productId}`);

export const deleteProductImage = (imageId) =>
  apiClient.delete(`/productImages/${imageId}`);
