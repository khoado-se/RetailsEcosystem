import { useNavigate } from "react-router-dom";
import Pagination from "../../components/ui/Pagination";
import { ENV } from "../../configs/env";

export default function ProductTable({ products, pageNumber, setPageNumber, totalPage }) {
  const navigate = useNavigate();

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
                      onClick={() => navigate(`/products/edit/${product.id}`)}
                      className="btn btn-primary btn-sm me-2"
                    >
                      <i className="bi bi-pencil me-1" />
                      Edit
                    </button>
                    <button
                      onClick={() => navigate(`/products/delete/${product.id}`)}
                      className="btn btn-danger btn-sm"
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
