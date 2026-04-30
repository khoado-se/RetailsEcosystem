import { useState } from "react";
import { createProduct } from "./productApi";
import ProductForm from "./ProductForm";

const EMPTY_FORM = {
  name: "",
  description: "",
  price: "",
  categoryId: "",
  isFeatured: false,
};

export default function CreateProductModal({ onSuccess }) {
  const [form, setForm] = useState(EMPTY_FORM);

  const handleChange = (e) => {
    const { name, type, value, checked } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = async () => {
    const payload = {
      name: form.name.trim(),
      description: form.description || null,
      price: Number(form.price),
      categoryId: Number(form.categoryId),
      isFeatured: form.isFeatured,
    };

    await createProduct(payload);
    setForm(EMPTY_FORM);
    onSuccess();
  };

  return (
    <div className="modal fade" id="createProductModal" tabIndex="-1" aria-hidden="true">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Create Product</h5>
            <button type="button" className="btn-close" data-bs-dismiss="modal" />
          </div>

          <ProductForm form={form} handleChange={handleChange} />

          <div className="modal-footer">
            <button className="btn btn-secondary" data-bs-dismiss="modal">
              Close
            </button>
            <button className="btn btn-primary" onClick={handleSubmit} data-bs-dismiss="modal">
              Create
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
