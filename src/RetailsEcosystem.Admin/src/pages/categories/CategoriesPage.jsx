import { useState } from "react";
import { useCategories } from "../../features/category/useCategories";
import CategoryTable from "../../features/category/CategoryTable";
import CategoryForm from "../../features/category/CategoryForm";
import { deleteCategory } from "../../features/category/categoryApi";

export default function CategoriesPage() {
  const { categories, fetchCategories } = useCategories();
  const [showForm, setShowForm] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredCategories = categories.filter((c) =>
    c.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleEdit = (cat) => {
    setSelectedCategory(cat);
    setShowForm(true);
  };

  const handleCreate = () => {
    setSelectedCategory(null);
    setShowForm(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this category?")) return;
    try {
      await deleteCategory(id);
      fetchCategories();
    } catch (err) {
      console.error("Delete failed:", err);
    }
  };

  const handleSuccess = () => {
    setShowForm(false);
    fetchCategories();
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
            {searchTerm && (
              <div className="col-auto">
                <button
                  className="btn btn-outline-secondary"
                  onClick={() => setSearchTerm("")}
                >
                  <i className="bi bi-x-lg me-1" />
                  Clear
                </button>
              </div>
            )}
          </div>
        </div>
      </div>

      <CategoryTable
        categories={filteredCategories}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />

      {showForm && (
        <CategoryForm
          category={selectedCategory}
          onSuccess={handleSuccess}
          onClose={() => setShowForm(false)}
        />
      )}
    </>
  );
}
