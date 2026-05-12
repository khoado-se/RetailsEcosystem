import { useState, useEffect, useRef } from "react";
import { Modal } from "bootstrap";
import { updateOrderStatus } from "./orderApi";
import { formatCurrency, formatDateTime } from "../../utils/format";

const STATUS_OPTIONS = [
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

const VALID_NEXT_STATUSES = {
  0: [1, 4], // Pending → Confirmed, Cancelled
  1: [2, 4], // Confirmed → Shipped, Cancelled
  2: [3],    // Shipped → Delivered
  3: [],     // Delivered — terminal
  4: [],     // Cancelled — terminal
};

const NEXT_STATUS = { 0: 1, 1: 2, 2: 3 };

const NEXT_STATUS_LABEL = {
  1: "Confirm Order",
  2: "Mark as Shipped",
  3: "Mark as Delivered",
};

const PAYMENT_METHOD_LABEL = { 0: "COD", 1: "VNPay" };
const PAYMENT_STATUS_LABEL = {
  0: "Pending",
  1: "Awaiting Payment",
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

export default function OrderDetailModal({ order, onStatusUpdated, onClose }) {
  const [pendingAction, setPendingAction] = useState(null);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const modalRef = useRef(null);
  const confirmModalRef = useRef(null);

  // Show/hide driven by order prop — single source of truth
  useEffect(() => {
    if (!modalRef.current) return;
    const instance = Modal.getOrCreateInstance(modalRef.current);
    if (order) {
      instance.show();
    } else {
      instance.hide();
    }
  }, [order]);

  // Sync all Bootstrap close paths (X, Escape, click-outside) back to React
  useEffect(() => {
    const el = modalRef.current;
    if (!el) return;
    const handler = () => onClose?.();
    el.addEventListener("hidden.bs.modal", handler);
    return () => el.removeEventListener("hidden.bs.modal", handler);
  }, [onClose]);

  useEffect(() => {
    if (order) setError(null);
  }, [order]);

  // Open/close confirmation modal when pendingAction changes
  useEffect(() => {
    if (!confirmModalRef.current) return;
    const instance = Modal.getOrCreateInstance(confirmModalRef.current);
    if (pendingAction) {
      instance.show();
    } else {
      instance.hide();
    }
  }, [pendingAction]);

  // Sync confirm modal Bootstrap close back to React
  useEffect(() => {
    const el = confirmModalRef.current;
    if (!el) return;
    const handler = () => setPendingAction(null);
    el.addEventListener("hidden.bs.modal", handler);
    return () => el.removeEventListener("hidden.bs.modal", handler);
  }, []);

  const isTerminal = order ? VALID_NEXT_STATUSES[order.status]?.length === 0 : false;

  // VNPay orders not yet paid must not be manually confirmed
  const allowedNextValues = order
    ? (order.paymentMethod === 1 && order.paymentStatus !== 2
        ? (VALID_NEXT_STATUSES[order.status] ?? []).filter((s) => s !== 1)
        : (VALID_NEXT_STATUSES[order.status] ?? []))
    : [];

  const nextStatus = order && allowedNextValues.includes(NEXT_STATUS[order.status])
    ? NEXT_STATUS[order.status]
    : null;

  const canCancel = allowedNextValues.includes(4);

  const handleConfirm = async () => {
    if (!pendingAction) return;
    setSaving(true);
    setError(null);
    try {
      await updateOrderStatus(order.id, pendingAction.targetStatus);
      Modal.getOrCreateInstance(confirmModalRef.current).hide();
      onStatusUpdated(STATUS_OPTIONS.find((s) => s.value === pendingAction.targetStatus)?.label);
    } catch (err) {
      setError(err.response?.data?.detail ?? "Failed to update status.");
      Modal.getOrCreateInstance(confirmModalRef.current).hide();
    } finally {
      setSaving(false);
      setPendingAction(null);
    }
  };

  return (
    <>
      <div className="modal fade" id="orderDetailModal" ref={modalRef} tabIndex={-1}>
        <div className="modal-dialog modal-lg modal-dialog-scrollable">
          <div className="modal-content">
            {order && (
              <>
                <div className="modal-header">
                  <h5 className="modal-title fw-bold">Order #{order.id}</h5>
                  <button type="button" className="btn-close" data-bs-dismiss="modal" />
                </div>

                <div className="modal-body">
                  {/* Meta */}
                  <div className="row g-3 mb-4">
                    <div className="col-sm-6">
                      <div className="text-muted small">Customer</div>
                      <div className="fw-semibold">{order.userEmail || order.userId}</div>
                    </div>
                    <div className="col-sm-6">
                      <div className="text-muted small">Placed</div>
                      <div className="fw-semibold">{formatDateTime(order.createdDate)}</div>
                    </div>
                    <div className="col-sm-6">
                      <div className="text-muted small">Shipping Address</div>
                      <div className="fw-semibold">{order.shippingAddress}</div>
                    </div>
                    <div className="col-sm-6">
                      <div className="text-muted small">Total</div>
                      <div className="fw-semibold">{formatCurrency(order.totalAmount)}</div>
                    </div>
                  </div>

                  {/* Payment info */}
                  <div className="row g-3 mb-4">
                    <div className="col-sm-6">
                      <div className="text-muted small">Payment Method</div>
                      <span className={`badge fw-medium ${order.paymentMethod === 1 ? "bg-info bg-opacity-10 text-info" : "bg-secondary bg-opacity-10 text-secondary"}`}>
                        {PAYMENT_METHOD_LABEL[order.paymentMethod] ?? "COD"}
                      </span>
                    </div>
                    <div className="col-sm-6">
                      <div className="text-muted small">Payment Status</div>
                      <span className={`badge fw-medium ${PAYMENT_STATUS_CLASS[order.paymentStatus] ?? "bg-secondary bg-opacity-10 text-secondary"}`}>
                        {PAYMENT_STATUS_LABEL[order.paymentStatus] ?? "Unknown"}
                      </span>
                    </div>
                    {order.vnpayTransactionNo && (
                      <div className="col-12">
                        <div className="text-muted small">VNPay Transaction No.</div>
                        <code className="small">{order.vnpayTransactionNo}</code>
                      </div>
                    )}
                  </div>

                  {/* Status update */}
                  <div className="mb-4">
                    <div className="text-muted small fw-semibold mb-2">Update Status</div>
                    <div className="d-flex align-items-center gap-2 flex-wrap">
                      <span className={`badge ${STATUS_BADGE[order.status]}`}>
                        {STATUS_OPTIONS.find((s) => s.value === order.status)?.label}
                      </span>
                      {isTerminal ? (
                        <span className="text-muted small">No further status changes allowed.</span>
                      ) : (
                        <>
                          {nextStatus !== null && (
                            <button
                              className="btn btn-primary btn-sm px-3"
                              onClick={() => setPendingAction({ targetStatus: nextStatus, label: NEXT_STATUS_LABEL[nextStatus] })}
                            >
                              {NEXT_STATUS_LABEL[nextStatus]} →
                            </button>
                          )}
                          {canCancel && (
                            <button
                              className="btn btn-outline-danger btn-sm px-3"
                              onClick={() => setPendingAction({ targetStatus: 4, label: "Cancel Order" })}
                            >
                              Cancel Order
                            </button>
                          )}
                        </>
                      )}
                    </div>
                    {error && <div className="text-danger small mt-2">{error}</div>}
                  </div>

                  {/* Items */}
                  <h6 className="fw-bold mb-3">Items</h6>
                  <table className="table table-sm align-middle">
                    <thead>
                      <tr>
                        <th>Product</th>
                        <th className="text-center">Qty</th>
                        <th className="text-center">Unit Price</th>
                        <th className="text-end">Total</th>
                      </tr>
                    </thead>
                    <tbody>
                      {order.items?.map((item) => (
                        <tr key={item.id}>
                          <td>{item.productName}</td>
                          <td className="text-center">{item.quantity}</td>
                          <td className="text-center">{formatCurrency(item.unitPrice)}</td>
                          <td className="text-end fw-semibold">{formatCurrency(item.lineTotal)}</td>
                        </tr>
                      ))}
                    </tbody>
                    <tfoot>
                      <tr className="fw-bold">
                        <td colSpan={3} className="text-end">Grand Total</td>
                        <td className="text-end">{formatCurrency(order.totalAmount)}</td>
                      </tr>
                    </tfoot>
                  </table>
                </div>

                <div className="modal-footer">
                  <button type="button" className="btn btn-outline-secondary" data-bs-dismiss="modal">
                    Close
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      </div>

      {/* Confirmation modal */}
      <div className="modal fade" id="orderStatusConfirmModal" ref={confirmModalRef} tabIndex={-1}>
        <div className="modal-dialog modal-sm modal-dialog-centered">
          <div className="modal-content">
            <div className="modal-header border-0 pb-0">
              <h6 className="modal-title fw-bold">Confirm Status Change</h6>
              <button type="button" className="btn-close" data-bs-dismiss="modal" />
            </div>
            <div className="modal-body pt-2">
              <p className="mb-0 small">
                Change order status from{" "}
                <strong>{STATUS_OPTIONS.find((s) => s.value === order?.status)?.label}</strong>{" "}
                to{" "}
                <strong>{STATUS_OPTIONS.find((s) => s.value === pendingAction?.targetStatus)?.label}</strong>?
              </p>
            </div>
            <div className="modal-footer border-0 pt-0">
              <button
                type="button"
                className="btn btn-sm btn-outline-secondary"
                data-bs-dismiss="modal"
              >
                Back
              </button>
              <button
                type="button"
                className={`btn btn-sm ${pendingAction?.targetStatus === 4 ? "btn-danger" : "btn-primary"}`}
                onClick={handleConfirm}
                disabled={saving}
              >
                {saving ? "Saving…" : "Confirm"}
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
