import apiClient from "../../services/apiClient";

export const uploadProductImage = (productId, formData) =>
  apiClient.post(`/productImages/upload?productId=${productId}`, formData, {
    // Do NOT set Content-Type manually — let the browser set multipart/form-data
    // with the correct boundary for FormData. Explicit override loses the boundary
    // and the server cannot parse the body.
    headers: { "Content-Type": undefined },
    timeout: 60000, // Cloudinary uploads can take several seconds
  });

export const getProductImages = (productId) =>
  apiClient.get(`/productImages/by-product/${productId}`);

export const deleteProductImage = (imageId) =>
  apiClient.delete(`/productImages/${imageId}`, {
    timeout: 30000, // Cloudinary deletion involves a remote API call
  });
