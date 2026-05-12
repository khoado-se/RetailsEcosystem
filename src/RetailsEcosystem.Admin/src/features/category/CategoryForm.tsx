import { useState } from "react";
import toast from "react-hot-toast";
import { createCategory, updateCategory } from "./categoryApi";

export default function CategoryForm({ category, onSuccess, onClose }) {
  const isEdit = category != null;
  const [name, setName] = useState(category?.name ?? "");
  const [description, setDescription] = useState(category?.description ?? "");
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!name.trim()) { setError("Category name is required."); return; }
    setError(null);
    setLoading(true);
    try {
      if (isEdit) {
        await updateCategory(category.id, { id: category.id, name, description: description || null });
        toast.success("Category updated successfully.");
      } else {
        await createCategory({ name, description: description || null });
        toast.success("Category created successfully.");
      }
      onSuccess();
    } catch (err) {
      setError(err.response?.data?.title || err.response?.data?.message || "Operation failed.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="modal-backdrop fade show" onClick={onClose} />
      <div className="modal fade show d-block" tabIndex={-1} role="dialog">
        <div className="modal-dialog modal-dialog-centered">
          <div className="modal-content card-panel">
            <div className="modal-header border-0 pb-0">
              <h5 className="modal-title text-section-title">
                {isEdit ? "Edit Category" : "Create Category"}
              </h5>
              <button type="button" className="btn-close" onClick={onClose} aria-label="Close" />
            </div>
            <form onSubmit={handleSubmit} noValidate>
              <div className="modal-body">
                {error && (
                  <div className="alert alert-danger d-flex align-items-start gap-2 py-2 px-3 mb-3" role="alert">
                    <i className="bi bi-exclamation-circle-fill flex-shrink-0 mt-1 text-caption" />
                    <span className="text-body-sm">{error}</span>
                  </div>
                )}
                <div className="mb-3">
                  <label htmlFor="cat-name" className="form-label text-label">Name</label>
                  <input
                    id="cat-name"
                    type="text"
                    className="form-control"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                    autoFocus
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="cat-description" className="form-label text-label">
                    Description <span className="text-muted fw-normal">(optional)</span>
                  </label>
                  <textarea
                    id="cat-description"
                    className="form-control"
                    rows={3}
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Brief description of this category…"
                  />
                </div>
              </div>
              <div className="modal-footer border-0 pt-0">
                <button type="button" className="btn btn-link text-muted" onClick={onClose}>
                  Cancel
                </button>
                <button type="submit" className="btn btn-primary" disabled={loading}>
                  {loading ? (
                    <span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true" />
                  ) : (
                    <i className="bi bi-floppy me-1" />
                  )}
                  {isEdit ? "Save changes" : "Create"}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </>
  );
}
