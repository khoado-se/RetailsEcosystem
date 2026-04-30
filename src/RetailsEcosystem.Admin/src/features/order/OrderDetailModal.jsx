import { useState } from "react";
import { updateOrderStatus } from "./orderApi";

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

export default function OrderDetailModal({ order, onStatusUpdated }) {
  const [selectedStatus, setSelectedStatus] = useState(order?.status ?? 0);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  if (!order) return null;

  const handleSave = async () => {
    setSaving(true);
    setError(null);
    try {
      await updateOrderStatus(order.id, selectedStatus);
      onStatusUpdated();
    } catch {
      setError("Failed to update status.");
    } finally {
      setSaving(false);
    }
  };

  const fmt = (date) =>
    date ? new Date(date).toLocaleString("en-US", { dateStyle: "medium", timeStyle: "short" }) : "—";

  return (
    <div className="modal fade" id="orderDetailModal" tabIndex={-1}>
      <div className="modal-dialog modal-lg modal-dialog-scrollable">
        <div className="modal-content">
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
                <div className="fw-semibold">{fmt(order.createdDate)}</div>
              </div>
              <div className="col-sm-6">
                <div className="text-muted small">Shipping Address</div>
                <div className="fw-semibold">{order.shippingAddress}</div>
              </div>
              <div className="col-sm-6">
                <div className="text-muted small">Total</div>
                <div className="fw-semibold">
                  {Number(order.totalAmount).toLocaleString("en-US", { style: "currency", currency: "USD" })}
                </div>
              </div>
            </div>

            {/* Status update */}
            <div className="mb-4">
              <label className="form-label fw-semibold small">Update Status</label>
              <div className="d-flex gap-2 align-items-center flex-wrap">
                <select
                  className="form-select"
                  style={{ maxWidth: 200 }}
                  value={selectedStatus}
                  onChange={(e) => setSelectedStatus(Number(e.target.value))}
                >
                  {STATUS_OPTIONS.map((s) => (
                    <option key={s.value} value={s.value}>{s.label}</option>
                  ))}
                </select>
                <button
                  className="btn btn-primary btn-sm px-3"
                  onClick={handleSave}
                  disabled={saving || selectedStatus === order.status}
                >
                  {saving ? "Saving…" : "Save"}
                </button>
                <span className={`badge ${STATUS_BADGE[order.status]}`}>
                  Current: {STATUS_OPTIONS.find(s => s.value === order.status)?.label}
                </span>
              </div>
              {error && <div className="text-danger small mt-1">{error}</div>}
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
                    <td className="text-center">
                      {Number(item.unitPrice).toLocaleString("en-US", { style: "currency", currency: "USD" })}
                    </td>
                    <td className="text-end fw-semibold">
                      {Number(item.lineTotal).toLocaleString("en-US", { style: "currency", currency: "USD" })}
                    </td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr className="fw-bold">
                  <td colSpan={3} className="text-end">Grand Total</td>
                  <td className="text-end">
                    {Number(order.totalAmount).toLocaleString("en-US", { style: "currency", currency: "USD" })}
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>

          <div className="modal-footer">
            <button type="button" className="btn btn-outline-secondary" data-bs-dismiss="modal">
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
