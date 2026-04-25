import apiClient from "../../services/apiClient";

// GET
export const getCategories = () => {
  return apiClient.get("/categories");
};

// GET by id
export const getCategoryById = (id) => {
  return apiClient.get(`/categories/${id}`);
};

// POST
export const createCategory = (data) => {
  return apiClient.post("/categories", data);
};

// PUT
export const updateCategory = (id, data) => {
  return apiClient.put(`/categories/${id}`, data);
};

// DELETE
export const deleteCategory = (id) => {
  return apiClient.delete(`/categories/${id}`);
};