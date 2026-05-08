import { useState, useEffect, useRef } from "react";
import { Modal } from "bootstrap";
import toast from "react-hot-toast";
import { getProductById, updateProduct } from "./productApi";
import { getProductImages, uploadProductImage, deleteProductImage } from "../productImage/productImageApi";
import ProductForm from "./ProductForm";
import { validateProductForm } from "./validateProductForm";

const ALLOWED_EXTS = [".jpg", ".jpeg", ".png", ".webp"];

export default function EditProductModal({ productId, onSuccess, onClose }) {
  // form=null means "not loaded yet" — used as the loading indicator
  const [form, setForm] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [existingImages, setExistingImages] = useState([]);
  const [pendingDeletes, setPendingDeletes] = useState([]);
  const [files, setFiles] = useState([]);
  const [previews, setPreviews] = useState([]);
  const modalRef = useRef(null);
  const fileInputRef = useRef(null);
  const savedRef = useRef(false);
  const onSuccessRef = useRef(onSuccess);
  useEffect(() => { onSuccessRef.current = onSuccess; }, [onSuccess]);

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

  // Sync Bootstrap's close events (X button, click-outside, Escape) back to React.
  // All state resets happen here so effects never call setState synchronously.
  // onSuccess is called after onClose so fetchProducts() fires once React has settled.
  useEffect(() => {
    const el = modalRef.current;
    if (!el) return;
    const handleHidden = () => {
      setForm(null);
      setFiles([]);
      setPreviews([]);
      setExistingImages([]);
      setPendingDeletes([]);
      const wasSaved = savedRef.current;
      savedRef.current = false;
      onClose?.();
      if (wasSaved) onSuccess?.();
    };
    el.addEventListener("hidden.bs.modal", handleHidden);
    return () => el.removeEventListener("hidden.bs.modal", handleHidden);
  }, [onClose, onSuccess]);

  // Load product data — no synchronous setState; form=null is the loading signal
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
          categoryName: p.category?.name ?? "",
          isFeatured: p.isFeatured ?? false,
        });
      })
      .catch(() => {});
    return () => { active = false; };
  }, [productId]);

  // Load existing images — state already reset in hidden.bs.modal handler
  useEffect(() => {
    if (!productId) return;
    getProductImages(productId)
      .then((res) => setExistingImages(res.data))
      .catch(() => {});
  }, [productId]);

  // Revoke object URLs to prevent memory leaks
  useEffect(() => {
    return () => previews.forEach((u) => URL.revokeObjectURL(u));
  }, [previews]);

  const handleChange = (e) => {
    const { name, type, value, checked } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleFileChange = (e) => {
    const selected = Array.from(e.target.files);
    const invalid = selected.filter(
      (f) => !ALLOWED_EXTS.some((ext) => f.name.toLowerCase().endsWith(ext))
    );
    if (invalid.length > 0) {
      toast.error(`Unsupported file type: ${invalid.map((f) => f.name).join(", ")}`);
      e.target.value = "";
      return;
    }
    setFiles(selected);
    setPreviews(selected.map((f) => URL.createObjectURL(f)));
  };

  const removeFile = (index) => {
    setFiles((prev) => prev.filter((_, i) => i !== index));
    setPreviews((prev) => {
      URL.revokeObjectURL(prev[index]);
      return prev.filter((_, i) => i !== index);
    });
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const markForDelete = (imageId) => {
    setPendingDeletes((prev) => [...prev, imageId]);
    setExistingImages((prev) => prev.filter((img) => img.id !== imageId));
  };

  const handleSubmit = async () => {
    if (!form) return;
    const errors = validateProductForm(form);
    if (errors.length) { toast.error(errors[0]); return; }

    const payload = {
      id: form.id,
      name: form.name.trim(),
      description: form.description || null,
      price: Number(form.price),
      categoryId: Number(form.categoryId),
      isFeatured: form.isFeatured,
    };

    setIsSubmitting(true);
    try {
      await updateProduct(form.id, payload);
    } catch {
      toast.error("Failed to update product.");
      setIsSubmitting(false);
      return;
    }
    setIsSubmitting(false);

    toast.success("Product updated successfully.");
    savedRef.current = true;

    // Capture before Modal.hide() triggers state reset
    const productId = form.id;
    const deletesToFire = [...pendingDeletes];
    const filesToFire = [...files];

    Modal.getOrCreateInstance(modalRef.current).hide();

    if (deletesToFire.length > 0) {
      Promise.all(deletesToFire.map((id) => deleteProductImage(id)))
        .then(() => {
          toast.success("Images deleted.");
          onSuccessRef.current?.();
        })
        .catch(() => toast.error("Some images could not be deleted."));
    }

    if (filesToFire.length > 0) {
      const formData = new FormData();
      filesToFire.forEach((f) => formData.append("files", f));
      uploadProductImage(productId, formData)
        .then(() => {
          toast.success("Images uploaded successfully.");
          onSuccessRef.current?.();
        })
        .catch(() => toast.error("Images could not be uploaded."));
    }
  };

  const imageSection = (
    <div className="mb-3">
      <label className="form-label">Images</label>

      {existingImages.length > 0 && (
        <div className="d-flex flex-wrap gap-2 mb-2">
          {existingImages.map((img) => (
            <div key={img.id} style={{ position: "relative" }}>
              <img
                src={img.url}
                alt=""
                style={{ width: 72, height: 72, objectFit: "cover", borderRadius: 6 }}
              />
              <button
                type="button"
                className="btn btn-danger btn-sm"
                style={{ position: "absolute", top: 2, right: 2, padding: "1px 4px", lineHeight: 1 }}
                onClick={() => markForDelete(img.id)}
                title="Remove image"
              >
                <i className="bi bi-x" />
              </button>
            </div>
          ))}
        </div>
      )}

      <input
        ref={fileInputRef}
        type="file"
        className="form-control mb-2"
        multiple
        accept=".jpg,.jpeg,.png,.webp"
        onChange={handleFileChange}
      />
      {previews.length > 0 && (
        <div className="d-flex flex-wrap gap-2 mt-2">
          {previews.map((src, i) => (
            <div key={i} style={{ position: "relative" }}>
              <img
                src={src}
                alt=""
                style={{ width: 72, height: 72, objectFit: "cover", borderRadius: 6, opacity: 0.85 }}
              />
              <button
                type="button"
                className="btn btn-danger btn-sm"
                style={{ position: "absolute", top: 2, right: 2, padding: "1px 4px", lineHeight: 1 }}
                onClick={() => removeFile(i)}
                title="Remove selection"
              >
                <i className="bi bi-x" />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );

  return (
    <div className="modal fade" id="editProductModal" ref={modalRef} tabIndex="-1" aria-hidden="true">
      <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Edit Product</h5>
            <button type="button" className="btn-close" data-bs-dismiss="modal" />
          </div>

          {!form ? (
            <div className="modal-body text-center py-4 text-muted">Loading…</div>
          ) : (
            <ProductForm form={form} handleChange={handleChange} imageSection={imageSection} />
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
