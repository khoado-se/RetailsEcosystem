import { useState } from "react";
import toast from "react-hot-toast";
import Pagination from "../../components/ui/Pagination";
import { ENV } from "../../configs/env";
import { deleteProduct } from "./productApi";
import ConfirmModal from "../../components/ui/ConfirmModal";

function SortHeader({ column, label, sortBy, sortDesc, onSort }) {
  const active = sortBy === column;
  return (
    <th style={{ cursor: "pointer", userSelect: "none", whiteSpace: "nowrap" }} onClick={() => onSort(column)}>
      {label}
      <i className={`bi ms-1 ${active ? (sortDesc ? "bi-arrow-down" : "bi-arrow-up") : "bi-arrow-down-up text-muted"}`} />
    </th>
  );
}

export default function ProductTable({ products, pageNumber, setPageNumber, totalPage, loading, onEdit, onDeleted, onImages, onClearFilters, sortBy, sortDesc, onSort }) {
  const [deleteTarget, setDeleteTarget] = useState(null);

  const handleDelete = (product) => setDeleteTarget(product);

  const handleDeleteConfirm = async () => {
    const product = deleteTarget;
    setDeleteTarget(null);
    try {
      await deleteProduct(product.id);
      toast.success("Product deleted.");
      onDeleted();
    } catch {
      toast.error("Failed to delete product.");
    }
  };

  if (!loading && products?.length === 0) {
    return (
      <>
        <div className="card border-0 shadow-sm mb-4">
          <div className="card-body text-center py-5">
            <i className="bi bi-search fs-2 text-muted mb-3 d-block" />
            <p className="fw-semibold mb-1">No products found</p>
            <p className="text-muted small mb-3">Try a different search term or category.</p>
            {onClearFilters && (
              <button className="btn btn-outline-primary btn-sm" onClick={onClearFilters}>
                Clear filters
              </button>
            )}
          </div>
        </div>
        <div className="d-flex justify-content-center mt-3">
          <Pagination pageNumber={pageNumber} setPageNumber={setPageNumber} totalPage={totalPage} />
        </div>
        {deleteTarget && (
          <ConfirmModal
            title="Delete Product"
            message={`Delete "${deleteTarget.name}"? This action cannot be undone.`}
            confirmLabel="Delete"
            onConfirm={handleDeleteConfirm}
            onClose={() => setDeleteTarget(null)}
          />
        )}
      </>
    );
  }

  return (
    <>
      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body p-0">
          <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <SortHeader column="id" label="Id" sortBy={sortBy} sortDesc={sortDesc} onSort={onSort} />
                <SortHeader column="name" label="Name" sortBy={sortBy} sortDesc={sortDesc} onSort={onSort} />
                <SortHeader column="price" label="Price" sortBy={sortBy} sortDesc={sortDesc} onSort={onSort} />
                <th>Featured</th>
                <th>Image</th>
                <SortHeader column="createdDate" label="Created Date" sortBy={sortBy} sortDesc={sortDesc} onSort={onSort} />
                <th className="text-end">Action</th>
              </tr>
            </thead>
            <tbody>
              {products?.map((product) => (
                <tr key={product.id}>
                  <td>{product.id}</td>
                  <td className="fw-semibold">{product.name}</td>
                  <td>{product.price}</td>
                  <td>
                    {product.isFeatured ? (
                      <span className="badge bg-primary bg-opacity-10 text-primary rounded-badge">Featured</span>
                    ) : (
                      <span className="text-muted small">—</span>
                    )}
                  </td>
                  <td>
                    <img
                      src={product.imageUrl || ENV.PRODUCT_PLACEHOLDER_IMAGE}
                      alt="Product"
                      width={50}
                      className="rounded-badge"
                    />
                  </td>
                  <td className="text-muted small">{product.createdDate}</td>
                  <td className="text-end">
                    <button
                      className="btn btn-outline-secondary btn-sm me-2"
                      title="Images"
                      onClick={() => onImages(product.id, product.name)}
                    >
                      <i className="bi bi-images" />
                      <span className="d-none d-sm-inline ms-1">Images</span>
                    </button>
                    <button
                      className="btn btn-primary btn-sm me-2"
                      title="Edit"
                      onClick={() => onEdit(product.id)}
                    >
                      <i className="bi bi-pencil" />
                      <span className="d-none d-sm-inline ms-1">Edit</span>
                    </button>
                    <button
                      className="btn btn-danger btn-sm"
                      title="Delete"
                      onClick={() => handleDelete(product)}
                    >
                      <i className="bi bi-trash" />
                      <span className="d-none d-sm-inline ms-1">Delete</span>
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        </div>
      </div>

      <div className="d-flex justify-content-center mt-3">
        <Pagination pageNumber={pageNumber} setPageNumber={setPageNumber} totalPage={totalPage} />
      </div>

      {deleteTarget && (
        <ConfirmModal
          title="Delete Product"
          message={`Delete "${deleteTarget.name}"? This action cannot be undone.`}
          confirmLabel="Delete"
          onConfirm={handleDeleteConfirm}
          onClose={() => setDeleteTarget(null)}
        />
      )}
    </>
  );
}
