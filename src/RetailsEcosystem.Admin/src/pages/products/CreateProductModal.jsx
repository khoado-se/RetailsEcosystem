import { useState } from "react";
import { createProduct } from "../../features/product/productApi";
import ProductForm from "../../features/product/ProductForm";

export default function CreateProductModal({ onSuccess }) {

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

  const handleSubmit = async () => {
    
    const payload = {
      ...form,
      name: form.name.trim(),
      price: Number(form.price),
      description: form.description || null,
      categoryId: Number(form.categoryId)
    };

    await createProduct(payload);

    onSuccess(); // reload list
  };

  return (
    <div
      className="modal fade"
      id="createProductModal"
      tabIndex="-1"
      aria-hidden="true"
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">

          {/* Header */}
          <div className="modal-header">
            <h5 className="modal-title">Create Product</h5>
            <button
              type="button"
              className="btn-close"
              data-bs-dismiss="modal"
            />
          </div>

          {/* Body */}
          <ProductForm form={form} handleChange={handleChange} />

          {/* Footer */}
          <div className="modal-footer">
            <button
              className="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Close
            </button>

            <button
              className="btn btn-primary"
              onClick={handleSubmit}
              data-bs-dismiss="modal"
            >
              Create
            </button>
          </div>

        </div>
      </div>
    </div>
  );
}