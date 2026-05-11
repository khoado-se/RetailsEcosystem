import { useState } from "react";
import toast from "react-hot-toast";
import { useOrders } from "../../features/order/useOrders";
import OrderDetailModal from "../../features/order/OrderDetailModal";
import Pagination from "../../components/ui/Pagination";
import PageSizeSelector from "../../components/ui/PageSizeSelector";
import { getOrderById } from "../../features/order/orderApi";
import { formatCurrency, formatDate } from "../../utils/format";

const STATUS_OPTIONS = [
  { value: null, label: "All" },
  { value: 0, label: "Pending" },
  { value: 1, label: "Confirmed" },
  { value: 2, label: "Shipped" },
  { value: 3, label: "Delivered" },
  { value: 4, label: "Cancelled" },
];

const STATUS_BADGE = {
  0: "bg-warning text-dark",
  1: "bg-info text-dark",
  2: "bg-primary",
  3: "bg-success",
  4: "bg-secondary",
};

const PAYMENT_STATUS_LABEL = {
  0: "Unpaid",
  1: "Awaiting",
  2: "Paid",
  3: "Failed",
  4: "Cancelled",
  5: "Expired",
  6: "Abandoned",
};

const PAYMENT_STATUS_CLASS = {
  0: "bg-secondary bg-opacity-10 text-secondary",
  1: "bg-warning bg-opacity-10 text-warning",
  2: "bg-success bg-opacity-10 text-success",
  3: "bg-danger bg-opacity-10 text-danger",
  4: "bg-secondary bg-opacity-10 text-secondary",
  5: "bg-secondary bg-opacity-10 text-secondary",
  6: "bg-secondary bg-opacity-10 text-secondary",
};

export default function OrdersPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [statusFilter, setStatusFilter] = useState(null);
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [loadingDetail, setLoadingDetail] = useState(false);

  const { orders, totalPage, loading, error, fetchOrders } = useOrders(pageNumber, statusFilter, pageSize);

  const handleStatusFilter = (val) => {
    setStatusFilter(val);
    setPageNumber(1);
  };

  const handlePageSizeChange = (size) => {
    setPageSize(size);
    setPageNumber(1);
  };

  const handleView = async (orderId) => {
    setLoadingDetail(true);
    try {
      const res = await getOrderById(orderId);
      setSelectedOrder(res.data);
    } catch {
      /* ignore */
    } finally {
      setLoadingDetail(false);
    }
  };

  const handleStatusUpdated = (label) => {
    fetchOrders();
    setSelectedOrder(null);
    toast.success(`Order status updated to ${label}.`);
  };


  return (
    <>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h1 className="page-header-title mb-0">Orders</h1>
          <p className="page-header-subtitle mb-0">Track and manage customer orders</p>
        </div>
      </div>

      {/* Status Filter + Rows per page */}
      <div className="mb-3 d-flex align-items-center justify-content-between flex-wrap gap-2">
        <div className="d-flex gap-2 flex-wrap">
          {STATUS_OPTIONS.map((s) => (
            <button
              key={s.label}
              className={`btn btn-sm rounded-pill ${statusFilter === s.value ? "btn-primary" : "btn-outline-secondary"}`}
              onClick={() => handleStatusFilter(s.value)}
            >
              {s.label}
            </button>
          ))}
        </div>
        <PageSizeSelector pageSize={pageSize} onPageSizeChange={handlePageSizeChange} />
      </div>

      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body p-0">
          {loading ? (
            <div className="text-center py-5 text-muted">Loading…</div>
          ) : error ? (
            <div className="text-center py-5 text-danger">{error}</div>
          ) : (
            <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead>
                <tr>
                  <th>Order #</th>
                  <th>Customer</th>
                  <th>Date</th>
                  <th className="text-center">Items</th>
                  <th className="text-end">Total</th>
                  <th className="text-center">Payment</th>
                  <th className="text-center">Status</th>
                  <th className="text-end">Action</th>
                </tr>
              </thead>
              <tbody>
                {orders.length === 0 ? (
                  <tr>
                    <td colSpan={8} className="text-center text-muted py-5">
                      No orders found.
                    </td>
                  </tr>
                ) : (
                  orders.map((order) => (
                    <tr key={order.id}>
                      <td className="fw-semibold">#{order.id}</td>
                      <td className="text-muted small">{order.userEmail || order.userId}</td>
                      <td className="text-muted small">{formatDate(order.createdDate)}</td>
                      <td className="text-center">{order.items?.length ?? 0}</td>
                      <td className="text-end fw-semibold">
                        {formatCurrency(order.totalAmount)}
                      </td>
                      <td className="text-center">
                        <div className="d-flex flex-column align-items-center gap-1">
                          <span className={`badge ${order.paymentMethod === 1 ? "bg-info bg-opacity-10 text-info" : "bg-secondary bg-opacity-10 text-secondary"}`}>
                            {order.paymentMethod === 1 ? "VNPay" : "COD"}
                          </span>
                          {order.paymentMethod === 1 && (
                            <span className={`badge ${PAYMENT_STATUS_CLASS[order.paymentStatus] ?? "bg-secondary bg-opacity-10 text-secondary"}`}>
                              {PAYMENT_STATUS_LABEL[order.paymentStatus] ?? "Unknown"}
                            </span>
                          )}
                        </div>
                      </td>
                      <td className="text-center">
                        <span className={`badge ${STATUS_BADGE[order.status]}`}>
                          {STATUS_OPTIONS.find((s) => s.value === order.status)?.label ?? order.statusLabel}
                        </span>
                      </td>
                      <td className="text-end">
                        <button
                          className="btn btn-sm btn-outline-primary"
                          disabled={loadingDetail}
                          onClick={() => handleView(order.id)}
                        >
                          <i className="bi bi-eye me-1" />
                          View
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
            </div>
          )}
        </div>
      </div>

      <div className="d-flex justify-content-center mt-3">
        <Pagination pageNumber={pageNumber} setPageNumber={setPageNumber} totalPage={totalPage} />
      </div>

      <OrderDetailModal
        order={selectedOrder}
        onStatusUpdated={handleStatusUpdated}
        onClose={() => setSelectedOrder(null)}
      />
    </>
  );
}
