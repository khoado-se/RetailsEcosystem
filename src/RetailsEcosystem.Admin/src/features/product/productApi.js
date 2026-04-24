import apiClient from "../../app/apiClient";

// GET
export const getProducts = async ({ pageNumber, pageSize = 8, categoryId }) => {
  console.log("CALL API WITH:", { pageNumber, pageSize, categoryId });
  const res = await apiClient.get("/products", {
    params: {
      pageNumber,
      pageSize,
      categoryId,
    },
  });

  return res.data;
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