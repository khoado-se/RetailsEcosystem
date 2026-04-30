import Pagination from "../../components/ui/Pagination";
import { ENV } from "../../configs/env";
import { deleteProduct } from "./productApi";

export default function ProductTable({ products, pageNumber, setPageNumber, totalPage, onEdit, onDeleted, onImages }) {

  const handleDelete = async (product) => {
    if (!window.confirm(`Delete "${product.name}"?`)) return;
    try {
      await deleteProduct(product.id);
      onDeleted();
    } catch {
      alert("Failed to delete product.");
    }
  };

  return (
    <>
      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body p-0">
          <table className="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th>Id</th>
                <th>Name</th>
                <th>Price</th>
                <th>Featured</th>
                <th>Image</th>
                <th>Created Date</th>
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
                      style={{ borderRadius: "6px" }}
                    />
                  </td>
                  <td className="text-muted small">{product.createdDate}</td>
                  <td className="text-end">
                    <button
                      className="btn btn-outline-secondary btn-sm me-2"
                      data-bs-toggle="modal"
                      data-bs-target="#productImageModal"
                      onClick={() => onImages(product.id, product.name)}
                    >
                      <i className="bi bi-images me-1" />
                      Images
                    </button>
                    <button
                      className="btn btn-primary btn-sm me-2"
                      data-bs-toggle="modal"
                      data-bs-target="#editProductModal"
                      onClick={() => onEdit(product.id)}
                    >
                      <i className="bi bi-pencil me-1" />
                      Edit
                    </button>
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => handleDelete(product)}
                    >
                      <i className="bi bi-trash me-1" />
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Pagination pageNumber={pageNumber} setPageNumber={setPageNumber} totalPage={totalPage} />
    </>
  );
}
