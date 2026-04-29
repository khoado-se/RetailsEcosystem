import { useState, useCallback } from "react";
import { useCustomers } from "../../features/customer/useCustomers";
import CustomerTable from "../../features/customer/CustomerTable";

export default function CustomerListPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");

  const { customers, totalPage, loading, fetchCustomers } = useCustomers(pageNumber, search);

  const handleSearch = (e) => {
    e.preventDefault();
    setPageNumber(1);
    setSearch(searchInput.trim());
  };

  const handleStatusChange = useCallback(() => {
    fetchCustomers();
  }, [fetchCustomers]);

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1 className="page-header-title mb-0">Customers</h1>
          <p className="page-header-subtitle mb-0">Manage customer accounts</p>
        </div>
        <form className="d-flex gap-2" onSubmit={handleSearch}>
          <input
            type="search"
            className="form-control"
            placeholder="Search by name or email…"
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
          />
          <button type="submit" className="btn btn-primary px-4">
            <i className="bi bi-search"></i>
          </button>
        </form>
      </div>

      {loading ? (
        <div className="text-center py-5 text-muted">
          <div className="spinner-border spinner-border-sm me-2" role="status" />
          Loading…
        </div>
      ) : (
        <CustomerTable
          customers={customers}
          pageNumber={pageNumber}
          setPageNumber={setPageNumber}
          totalPage={totalPage}
          onStatusChange={handleStatusChange}
        />
      )}
    </>
  );
}
