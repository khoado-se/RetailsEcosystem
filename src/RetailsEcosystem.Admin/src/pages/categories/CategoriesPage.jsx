import { useState } from "react";
import toast from "react-hot-toast";
import { useCategories } from "../../features/category/useCategories";
import CategoryTable from "../../features/category/CategoryTable";
import CategoryForm from "../../features/category/CategoryForm";
import { deleteCategory } from "../../features/category/categoryApi";
import Pagination from "../../components/ui/Pagination";
import PageSizeSelector from "../../components/ui/PageSizeSelector";
import ConfirmModal from "../../components/ui/ConfirmModal";

export default function CategoriesPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [showForm, setShowForm] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [deleteTarget, setDeleteTarget] = useState(null);

  const { categories, totalPage, loading, error, fetchCategories } =
    useCategories(pageNumber, pageSize);

  const filteredCategories = categories.filter((c) =>
    c.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  const handleEdit = (cat) => {
    setSelectedCategory(cat);
    setShowForm(true);
  };

  const handleCreate = () => {
    setSelectedCategory(null);
    setShowForm(true);
  };

  const handleDelete = (id) => setDeleteTarget(id);

  const handleDeleteConfirm = async () => {
    const id = deleteTarget;
    setDeleteTarget(null);
    try {
      await deleteCategory(id);
      toast.success("Category deleted.");
      fetchCategories();
    } catch {
      toast.error("Failed to delete category.");
    }
  };

  const handleSuccess = () => {
    setShowForm(false);
    fetchCategories();
  };

  const handlePageSizeChange = (size) => {
    setPageSize(size);
    setPageNumber(1);
  };

  return (
    <>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h1 className="page-header-title mb-0">Categories</h1>
          <p className="page-header-subtitle mb-0">Manage product categories</p>
        </div>
        <button className="btn btn-primary" onClick={handleCreate}>
          <i className="bi bi-plus-lg me-1" />
          Create Category
        </button>
      </div>

      {/* Search Bar */}
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
                  placeholder="Search categories by name..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
            </div>

            <div className="col-12 col-md-3">
              {searchTerm && (
                <button
                  className="btn btn-outline-secondary"
                  onClick={() => setSearchTerm("")}
                >
                  <i className="bi bi-x-lg me-1" />
                  Clear
                </button>
              )}
            </div>
            <div className="col-12 col-md-3 d-flex justify-content-end">
              <PageSizeSelector
                pageSize={pageSize}
                onPageSizeChange={handlePageSizeChange}
              />
            </div>
          </div>
        </div>
      </div>

      {error && (
        <div
          className="alert alert-danger d-flex align-items-start gap-2 rounded-3 mb-3"
          role="alert"
        >
          <i className="bi bi-exclamation-circle-fill flex-shrink-0 mt-1" />
          <div>
            {error}
            <button
              className="btn btn-sm btn-link p-0 ms-2"
              onClick={fetchCategories}
            >
              Retry
            </button>
          </div>
        </div>
      )}

      {loading ? (
        <div className="text-center py-5 text-muted">
          <div
            className="spinner-border spinner-border-sm me-2"
            role="status"
          />
          Loading…
        </div>
      ) : (
        <CategoryTable
          categories={filteredCategories}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      )}

      <div className="d-flex justify-content-center mt-3">
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={setPageNumber}
          totalPage={totalPage}
        />
      </div>

      {showForm && (
        <CategoryForm
          category={selectedCategory}
          onSuccess={handleSuccess}
          onClose={() => setShowForm(false)}
        />
      )}

      {deleteTarget !== null && (
        <ConfirmModal
          title="Delete Category"
          message="Delete this category? This action cannot be undone."
          confirmLabel="Delete"
          onConfirm={handleDeleteConfirm}
          onClose={() => setDeleteTarget(null)}
        />
      )}
    </>
  );
}
