import { useState, useEffect, useCallback } from "react";
import { getProducts } from "./productApi";

export const useProducts = (pageNumber, categoryId) => {
  const [data, setData] = useState({
    items: [],
    totalPage: 0,
  });

  const fetchProducts = useCallback(async () => {
    try {
      const res = await getProducts({pageNumber, categoryId });
      setData({
        items: res.items ?? [],
        totalPage: res.totalPage ?? 0,
      });
    } catch (err) {
      console.error("Fetch error:", err);
    }
  }, [pageNumber, categoryId]); // Only changes when these params change

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  return {
    products: data.items,
    totalPage: data.totalPage,
    refetch: fetchProducts,
  };
};