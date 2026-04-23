import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createProduct } from "../../features/product/productApi";
import { useCategories } from "../../features/category/useCategories";

export default function CreateProductPage() {
  const { categories } = useCategories();
  const navigate = useNavigate();

  const [form, setForm] = useState({
    name: "",
    description: "",
    price: "",
    categoryId: "",
  });

  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      ...form,
      price: Number(form.price),
      categoryId: Number(form.categoryId),
      createdDate: new Date().toISOString(),
      updatedDate: new Date().toISOString(),
    };

    await createProduct(payload);

    navigate("/products");
  };

  return (
    <div className="container mt-4">
      <div className="card p-4">
        <h3 className="mb-3">Create Product</h3>

        <form onSubmit={handleSubmit}>
          {/* Name */}
          <div className="mb-3">
            <label className="form-label">Name</label>
            <input
              name="name"
              className="form-control"
              value={form.name}
              onChange={handleChange}
              required
            />
          </div>

          {/* Price */}
          <div className="mb-3">
            <label className="form-label">Price</label>
            <input
              type="number"
              name="price"
              className="form-control"
              value={form.price}
              onChange={handleChange}
              required
            />
          </div>

          {/* Category */}
          <div className="mb-3">
            <label className="form-label">Category</label>
            <select
              name="categoryId"
              className="form-select"
              value={form.categoryId}
              onChange={handleChange}
              required
            >
              <option value="">-- Select category --</option>
              {
                categories.map((category) => (
                  <option key={category.id} value={category.id}>
                    {category.categoryName}
                  </option>
                ))
              }
            </select>
          </div>

          {/* Description */}
          <div className="mb-3">
            <label className="form-label">Description</label>
            <textarea
              name="description"
              className="form-control"
              rows="4"
              value={form.description}
              onChange={handleChange}
            />
          </div>

          {/* Actions */}
          <div className="d-flex gap-2">
            <button type="submit" className="btn btn-success">
              Create
            </button>

            <button
              type="button"
              className="btn btn-secondary"
              onClick={() => navigate("/products")}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
