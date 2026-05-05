import { useState, useEffect, useCallback } from "react";
import { getProducts } from "./productApi";

export const useProducts = (pageNumber, categoryId, search, pageSize = 10) => {
  const [data, setData] = useState({
    items: [],
    totalPage: 0,
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchProducts = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await getProducts({ pageNumber, pageSize, categoryId, search });
      setData({
        items: res.items ?? [],
        totalPage: res.totalPage ?? 0,
      });
    } catch (err) {
      setError(err.response?.data?.title || err.message || "Failed to load products.");
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize, categoryId, search]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  return {
    products: data.items,
    totalPage: data.totalPage,
    loading,
    error,
    fetchProducts,
  };
};
