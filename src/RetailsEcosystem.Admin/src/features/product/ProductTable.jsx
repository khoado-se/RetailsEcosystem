import { useNavigate } from "react-router-dom";
import Pagination from "../../pages/products/Pagination";

export default function ProductTable({products, page , setPage, totalPage}) {
    const navigate = useNavigate();

    return (<>
    <div className="container-fluid mt-4">
        <button
          className="btn btn-success mb-3"
          data-bs-toggle="modal"
          data-bs-target="#createProductModal"
        >
          + Create Product
        </button>

        {/* TABLE */}
        <table className="table table-striped">
          <thead>
            <tr>
              <th>Id</th>
              <th>Name</th>
              <th>Price</th>
              <th>Image</th>
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
                <td><img src={product.image.url} alt="Product Image" width={50} /></td>
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
      </div>
    </>)

};
