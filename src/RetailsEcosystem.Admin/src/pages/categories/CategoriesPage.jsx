import { useState } from "react";
import { useCategories } from "../../features/category/useCategories";
import CategoryTable from "../../features/category/CategoryTable";
import CategoryForm from "../../features/category/CategoryForm";
import { deleteCategory } from "../../features/category/categoryApi";

export default function CategoriesPage() {
  const { categories, fetchCategories } = useCategories();
  const [showForm, setShowForm] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);

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

      <CategoryTable
        categories={categories}
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
