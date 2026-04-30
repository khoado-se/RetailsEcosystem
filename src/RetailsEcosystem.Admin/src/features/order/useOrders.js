import { useState, useEffect, useCallback } from "react";
import { getOrders } from "./orderApi";

export function useOrders(pageNumber = 1, statusFilter = null) {
  const [orders, setOrders] = useState([]);
  const [totalPage, setTotalPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchOrders = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await getOrders({ pageNumber, pageSize: 10, status: statusFilter });
      setOrders(res.data.items ?? []);
      setTotalPage(res.data.totalPage ?? 1);
    } catch (err) {
      setError(err.message ?? "Failed to load orders.");
    } finally {
      setLoading(false);
    }
  }, [pageNumber, statusFilter]);

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  return { orders, totalPage, loading, error, fetchOrders };
}
