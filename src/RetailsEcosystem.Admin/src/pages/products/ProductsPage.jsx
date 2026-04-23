import { useProducts } from "../../features/product/useProducts.js";
import { useNavigate } from "react-router-dom";
import { Link, Outlet } from "react-router-dom";
import { useState } from "react";
import Pagination from "./Pagination.jsx";

export default function ProductsPage() {
  const [page, setPage] = useState(1);

  const { products, totalPage } = useProducts(page);


  const navigate = useNavigate();

  return (
    <>
      <Outlet />

      <div className="d-flex justify-content-between mb-3">
        <h3>Products</h3>
        <Link className="btn btn-success" to="create">
          + Create Product
        </Link>
      </div>

      {/* TABLE */}
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Price</th>
            <th>Description</th>
            <th>Created Date</th>
            <th>Action</th>
          </tr>
        </thead>

        <tbody>
          {products?.map((product) => (
            <tr key={product.id}>
              <td>{product.id}</td>
              <td>{product.name}</td>
              <td>{product.price}</td>
              <td>{product.description}</td>
              <td>{product.createdDate}</td>
              <td>
                <button
                  onClick={() => navigate(`/products/edit/${product.id}`)}
                  className="btn btn-primary btn-sm me-2"
                >
                  Edit
                </button>

                <button
                  onClick={() => navigate(`/products/delete/${product.id}`)}
                  className="btn btn-danger btn-sm"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* PAGINATION */}
      <Pagination page={page} setPage={setPage} totalPage={totalPage} />
    </>
  );
}
