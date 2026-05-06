import { useState, useEffect, useRef } from "react";
import { Modal } from "bootstrap";
import toast from "react-hot-toast";
import { getProductById, updateProduct } from "./productApi";
import ProductForm from "./ProductForm";
import { validateProductForm } from "./validateProductForm";

export default function EditProductModal({ productId, onSuccess, onClose }) {
  const [form, setForm] = useState(null);
  const [loading, setLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const modalRef = useRef(null);

  // Show/hide modal in response to productId — single source of truth
  useEffect(() => {
    if (!modalRef.current) return;
    const instance = Modal.getOrCreateInstance(modalRef.current);
    if (productId) {
      instance.show();
    } else {
      instance.hide();
    }
  }, [productId]);

  // Sync Bootstrap's close events (X button, click-outside, Escape) back to React
  useEffect(() => {
    const el = modalRef.current;
    if (!el) return;
    const handleHidden = () => onClose?.();
    el.addEventListener("hidden.bs.modal", handleHidden);
    return () => el.removeEventListener("hidden.bs.modal", handleHidden);
  }, [onClose]);

  useEffect(() => {
    if (!productId) return;
    let active = true;
    setLoading(true);
    setForm(null);
    getProductById(productId)
      .then((res) => {
        if (!active) return;
        const p = res.data;
        setForm({
          id: p.id,
          name: p.name ?? "",
          description: p.description ?? "",
          price: p.price ?? "",
          categoryId: p.category?.id ?? "",
          categoryName: p.category?.name ?? "",
          isFeatured: p.isFeatured ?? false,
        });
        setLoading(false);
      })
      .catch(() => active && setLoading(false));
    return () => { active = false; };
  }, [productId]);

  const handleChange = (e) => {
    const { name, type, value, checked } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleSubmit = async () => {
    if (!form) return;
    const errors = validateProductForm(form);
    if (errors.length) { toast.error(errors[0]); return; }

    const categoryId = Number(form.categoryId);
    const payload = {
      id: form.id,
      name: form.name.trim(),
      description: form.description || null,
      price: Number(form.price),
      categoryId,
      isFeatured: form.isFeatured,
    };

    setIsSubmitting(true);
    try {
      await updateProduct(form.id, payload);
      toast.success("Product updated successfully.");
      onSuccess();
      Modal.getOrCreateInstance(modalRef.current).hide();
    } catch {
      toast.error("Failed to update product.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="modal fade" id="editProductModal" ref={modalRef} tabIndex="-1" aria-hidden="true">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Edit Product</h5>
            <button type="button" className="btn-close" data-bs-dismiss="modal" />
          </div>

          {loading || !form ? (
            <div className="modal-body text-center py-4 text-muted">Loading…</div>
          ) : (
            <ProductForm form={form} handleChange={handleChange} />
          )}

          <div className="modal-footer">
            <button className="btn btn-secondary" data-bs-dismiss="modal">
              Close
            </button>
            <button
              className="btn btn-primary"
              onClick={handleSubmit}
              disabled={!form || isSubmitting}
            >
              {isSubmitting && (
                <span className="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true" />
              )}
              Save Changes
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
