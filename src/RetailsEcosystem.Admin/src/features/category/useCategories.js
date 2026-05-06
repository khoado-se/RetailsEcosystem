import { useState, useEffect, useCallback, useRef } from "react";
import { getCategories } from "./categoryApi";

export const useCategories = (pageNumber = 1, pageSize = 10) => {
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

export const useCategoriesInfinite = (pageSize = 10) => {
  const [items, setItems] = useState([]);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const pageRef = useRef(1);

  const loadMore = useCallback(async () => {
    if (loading) return;
    setLoading(true);
    setError(null);
    try {
      const currentPage = pageRef.current;
      const res = await getCategories({ pageNumber: currentPage, pageSize });
      const data = res.data;
      const fetched = data.items ?? [];
      const total = data.totalPage ?? 1;
      setItems((prev) => currentPage === 1 ? fetched : [...prev, ...fetched]);
      setHasMore(currentPage < total);
      pageRef.current = currentPage + 1;
      setPage(currentPage + 1);
    } catch (err) {
      setError(err.response?.data?.title || err.message || "Failed to load categories.");
    } finally {
      setLoading(false);
    }
  }, [loading, pageSize]);

  const reset = useCallback(() => {
    setItems([]);
    setPage(1);
    setHasMore(true);
    setError(null);
    pageRef.current = 1;
  }, []);

  return { items, page, hasMore, loading, error, loadMore, reset };
};
