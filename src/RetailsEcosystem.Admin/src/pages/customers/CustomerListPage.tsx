import { useState, useCallback, useEffect } from "react";
import { useCustomers } from "../../features/customer/useCustomers";
import CustomerTable from "../../features/customer/CustomerTable";
import PageSizeSelector from "../../components/ui/PageSizeSelector";

export default function CustomerListPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [searchInput, setSearchInput] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");

  const { customers, totalPage, loading, fetchCustomers } = useCustomers(pageNumber, debouncedSearch, pageSize);

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchInput);
      setPageNumber(1);
    }, 300);
    return () => clearTimeout(timer);
  }, [searchInput]);

  const handleClearSearch = () => {
    setSearchInput("");
    setDebouncedSearch("");
    setPageNumber(1);
  };

  const handlePageSizeChange = (size) => {
    setPageSize(size);
    setPageNumber(1);
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
      </div>

      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body py-3">
          <div className="row g-2 align-items-center">
            <div className="col-12 col-md-6">
              <div className="input-group">
                <span className="input-group-text bg-white border-end-0">
                  <i className="bi bi-search text-muted" />
                </span>
                <input
                  type="text"
                  className="form-control border-start-0"
                  placeholder="Search customers by name or email..."
                  value={searchInput}
                  onChange={(e) => setSearchInput(e.target.value)}
                />
              </div>
            </div>
            <div className="col-12 col-md-3">
              {searchInput && (
                <button className="btn btn-outline-secondary" onClick={handleClearSearch}>
                  <i className="bi bi-x-lg me-1" />
                  Clear
                </button>
              )}
            </div>
            <div className="col-12 col-md-3 d-flex justify-content-end">
              <PageSizeSelector pageSize={pageSize} onPageSizeChange={handlePageSizeChange} />
            </div>
          </div>
        </div>
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
