import { useState, useEffect, useCallback } from "react";
import { getCustomers } from "./customerApi";

export const useCustomers = (pageNumber, search, pageSize = 10) => {
  const [data, setData] = useState({ items: [], totalPage: 0 });
  const [loading, setLoading] = useState(false);

  const fetchCustomers = useCallback(async () => {
    setLoading(true);
    try {
      const res = await getCustomers({ pageNumber, pageSize, search });
      setData({ items: res.items ?? [], totalPage: res.totalPage ?? 0 });
    } catch (err) {
      console.error("Failed to fetch customers:", err);
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize, search]);

  useEffect(() => {
    fetchCustomers();
  }, [fetchCustomers]);

  return { customers: data.items, totalPage: data.totalPage, loading, fetchCustomers };
};
