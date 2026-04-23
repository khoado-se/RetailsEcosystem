import { useCategories } from "../../features/category/useCategories";
import { useNavigate } from "react-router-dom";

export default function CategoryListPage() {
  const { categories } = useCategories();
  const navigate = useNavigate();

  return (
    <>
      <div className="d-flex justify-content-between mb-3">
        <h3>Categoies</h3>
        <button
          className="btn btn-success"
          onClick={() => navigate("/category/create")}
        >
          + Create Category
        </button>
      </div>

      <table className="table table-striped">
        <thead>
          <tr>
            <th scope="col">Id</th>
            <th scope="col">Name</th>
            <th scope="col">Action</th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr key={category.id}>
              <th scope="row">{category.id}</th>
              <td>{category.categoryName}</td>
              <td>
                <button
                  onClick={() => navigate(`/categories/edit/${category.id}`)}
                  className="btn btn-primary btn-sm me-2"
                >
                  Edit
                </button>
                <button
                  onClick={() => navigate(`/categories/delete/${category.id}`)}
                  className="btn btn-danger btn-sm"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}
