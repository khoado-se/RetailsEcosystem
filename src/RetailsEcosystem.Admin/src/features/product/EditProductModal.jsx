import { useState, useEffect } from "react";
import { getProductById, updateProduct } from "./productApi";
import ProductForm from "./ProductForm";

export default function EditProductModal({ productId, onSuccess }) {
  const [form, setForm] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!productId) return;
    let active = true;
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
    const payload = {
      id: form.id,
      name: form.name.trim(),
      description: form.description || null,
      price: Number(form.price),
      categoryId: Number(form.categoryId),
      isFeatured: form.isFeatured,
    };
    await updateProduct(form.id, payload);
    onSuccess();
  };

  return (
    <div className="modal fade" id="editProductModal" tabIndex="-1" aria-hidden="true">
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
              data-bs-dismiss="modal"
              disabled={!form}
            >
              Save Changes
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
