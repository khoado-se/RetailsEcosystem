import { useState } from "react";
import Pagination from "../../components/ui/Pagination";
import { updateCustomerStatus } from "./customerApi";

export default function CustomerTable({ customers, pageNumber, setPageNumber, totalPage, onStatusChange }) {
  const [updatingId, setUpdatingId] = useState(null);

  const handleToggleStatus = async (customer) => {
    setUpdatingId(customer.id);
    try {
      await updateCustomerStatus(customer.id, !customer.isActive);
      onStatusChange();
    } catch (err) {
      console.error("Failed to update status:", err);
    } finally {
      setUpdatingId(null);
    }
  };

  return (
    <div className="container-fluid mt-4">
      <table className="table table-hover align-middle">
        <thead className="table-light">
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Phone</th>
            <th>Roles</th>
            <th>Status</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {customers.length === 0 ? (
            <tr>
              <td colSpan={6} className="text-center text-muted py-4">
                No customers found.
              </td>
            </tr>
          ) : (
            customers.map((c) => (
              <tr key={c.id}>
                <td className="fw-semibold">{c.fullName}</td>
                <td className="text-muted small">{c.email}</td>
                <td className="text-muted small">{c.phoneNumber || "—"}</td>
                <td>
                  {c.roles?.map((r) => (
                    <span key={r} className="badge bg-secondary me-1">{r}</span>
                  ))}
                </td>
                <td>
                  <span className={`badge rounded-pill ${c.isActive ? "bg-success" : "bg-danger"}`}>
                    {c.isActive ? "Active" : "Inactive"}
                  </span>
                </td>
                <td>
                  <button
                    className={`btn btn-sm rounded-pill ${c.isActive ? "btn-outline-danger" : "btn-outline-success"}`}
                    disabled={updatingId === c.id}
                    onClick={() => handleToggleStatus(c)}
                  >
                    {updatingId === c.id
                      ? "..."
                      : c.isActive ? "Deactivate" : "Activate"}
                  </button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      <Pagination pageNumber={pageNumber} setPageNumber={setPageNumber} totalPage={totalPage} />
    </div>
  );
}
