import { useCategories } from "../../features/category/useCategories";

export default function ProductForm({ form, handleChange }) {
  const { categories } = useCategories();

  return (
    <>
      <div className="modal-body">
        <div className="mb-3">
          <label>Name</label>
          <input
            name="name"
            className="form-control"
            value={form.name}
            onChange={handleChange}
          />
        </div>

        <div className="mb-3">
          <label>Price</label>
          <input
            type="number"
            name="price"
            className="form-control"
            value={form.price}
            onChange={handleChange}
          />
        </div>

        <div className="mb-3">
          <label>Category</label>
          <select
            name="categoryId"
            className="form-select"
            value={form.categoryId}
            onChange={handleChange}
          >
            <option value="">-- Select --</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </div>

        <div className="mb-3">
          <label>Description</label>
          <textarea
            name="description"
            className="form-control"
            value={form.description}
            onChange={handleChange}
          />
        </div>

        <div className="mb-3 form-check">
          <input
            type="checkbox"
            className="form-check-input"
            id="isFeatured"
            name="isFeatured"
            checked={!!form.isFeatured}
            onChange={handleChange}
          />
          <label className="form-check-label" htmlFor="isFeatured">
            Featured Product
          </label>
        </div>
      </div>
    </>
  );
}
