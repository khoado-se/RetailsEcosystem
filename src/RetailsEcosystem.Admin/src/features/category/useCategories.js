import { useState, useEffect, useCallback } from "react";
import { getCategories } from "./categoryApi";

export const useCategories = (pageNumber = 1, pageSize = 40) => {
  const [categories, setCategories] = useState([]);
  const [totalPage, setTotalPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchCategories = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await getCategories({ pageNumber, pageSize });
      setCategories(res.data.items ?? []);
      setTotalPage(res.data.totalPage ?? 1);
    } catch (err) {
      setError(err.response?.data?.title || err.message || "Failed to load categories.");
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize]);

  useEffect(() => { fetchCategories(); }, [fetchCategories]);

  return { categories, totalPage, loading, error, fetchCategories };
};
