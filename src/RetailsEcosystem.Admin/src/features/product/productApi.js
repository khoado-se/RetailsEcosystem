import apiClient from "../../app/apiClient";

// GET
export const getProducts = (pageNumber) => {
  return apiClient.get("/products?pageNumber="+pageNumber);
};

// GET by id
export const getProductById = (id) => {
  return apiClient.get(`/products/${id}`);
};

// POST
export const createProduct = (data) => {
  return apiClient.post("/products", data);
};

// PUT
export const updateProduct = (id, data) => {
  return apiClient.put(`/products/${id}`, data);
};

// DELETE
export const deleteProduct = (id) => {
  return apiClient.delete(`/products/${id}`);
};