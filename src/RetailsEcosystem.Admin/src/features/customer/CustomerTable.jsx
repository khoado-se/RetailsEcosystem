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
    <>
      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body p-0">
          <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Phone</th>
                <th>Roles</th>
                <th>Status</th>
                <th className="text-end">Action</th>
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
                        <span key={r} className="badge bg-primary bg-opacity-10 text-primary rounded-badge fw-medium me-1">{r}</span>
                      ))}
                    </td>
                    <td>
                      <span className={`badge fw-medium rounded-badge ${c.isActive
                        ? "bg-success bg-opacity-10 text-success"
                        : "bg-danger bg-opacity-10 text-danger"}`}>
                        {c.isActive ? "Active" : "Inactive"}
                      </span>
                    </td>
                    <td className="text-end">
                      <button
                        className={`btn btn-sm ${c.isActive ? "btn-outline-danger" : "btn-outline-success"}`}
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
        </div>
        </div>
      </div>

      <div className="d-flex justify-content-center mt-3">
        <Pagination pageNumber={pageNumber} setPageNumber={setPageNumber} totalPage={totalPage} />
      </div>
    </>
  );
}
