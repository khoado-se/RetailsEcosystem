import { useState, useRef, useEffect } from "react";
import { Modal } from "bootstrap";
import toast from "react-hot-toast";
import { createProduct } from "./productApi";
import ProductForm from "./ProductForm";

const EMPTY_FORM = {
  name: "",
  description: "",
  price: "",
  categoryId: "",
  isFeatured: false,
};

export default function CreateProductModal({ onSuccess, onClose }) {
  const [form, setForm] = useState(EMPTY_FORM);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const modalRef = useRef(null);

  // Reset form and notify parent on any close path (X, Escape, click-outside, submit)
  useEffect(() => {
    const el = modalRef.current;
    if (!el) return;
    const handler = () => { setForm(EMPTY_FORM); onClose?.(); };
    el.addEventListener("hidden.bs.modal", handler);
    return () => el.removeEventListener("hidden.bs.modal", handler);
  }, [onClose]);

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

    setIsSubmitting(true);
    try {
      await createProduct(payload);
      toast.success("Product created successfully.");
      onSuccess();
      Modal.getOrCreateInstance(modalRef.current).hide();
    } catch {
      toast.error("Failed to create product.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="modal fade" id="createProductModal" ref={modalRef} tabIndex="-1" aria-hidden="true">
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
            <button
              className="btn btn-primary"
              onClick={handleSubmit}
              disabled={isSubmitting}
            >
              {isSubmitting && (
                <span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true" />
              )}
              Create
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
