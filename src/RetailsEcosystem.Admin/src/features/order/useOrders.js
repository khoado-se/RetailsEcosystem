import { useState, useEffect, useCallback } from "react";
import { getOrders } from "./orderApi";

export function useOrders(pageNumber = 1, statusFilter = null, pageSize = 10) {
  const [orders, setOrders] = useState([]);
  const [totalPage, setTotalPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchOrders = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await getOrders({ pageNumber, pageSize, status: statusFilter });
      setOrders(res.data.items ?? []);
      setTotalPage(res.data.totalPage ?? 1);
    } catch (err) {
      setError(err.message ?? "Failed to load orders.");
    } finally {
      setLoading(false);
    }
  }, [pageNumber, pageSize, statusFilter]);

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  return { orders, totalPage, loading, error, fetchOrders };
}
