import { useState, useRef, useEffect } from "react";
import { Modal } from "bootstrap";
import toast from "react-hot-toast";
import { createProduct } from "./productApi";
import { uploadProductImage } from "../productImage/productImageApi";
import ProductForm from "./ProductForm";
import { validateProductForm } from "./validateProductForm";

const EMPTY_FORM = {
  name: "",
  description: "",
  price: "",
  categoryId: "",
  isFeatured: false,
};

const ALLOWED_EXTS = [".jpg", ".jpeg", ".png", ".webp"];

export default function CreateProductModal({ isOpen, onSuccess, onClose }) {
  const [form, setForm] = useState(EMPTY_FORM);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [files, setFiles] = useState([]);
  const [previews, setPreviews] = useState([]);
  const modalRef = useRef(null);
  const fileInputRef = useRef(null);
  const onSuccessRef = useRef(onSuccess);
  useEffect(() => { onSuccessRef.current = onSuccess; }, [onSuccess]);

  // Drive Bootstrap modal open/close from isOpen prop — no data-API involved
  useEffect(() => {
    if (!modalRef.current) return;
    const instance = Modal.getOrCreateInstance(modalRef.current);
    if (isOpen) {
      instance.show();
    } else {
      instance.hide();
    }
  }, [isOpen]);

  // Reset form and notify parent on any close path (X, Escape, click-outside)
  useEffect(() => {
    const el = modalRef.current;
    if (!el) return;
    const handler = () => {
      setForm(EMPTY_FORM);
      setFiles([]);
      setPreviews([]);
      onClose?.();
    };
    el.addEventListener("hidden.bs.modal", handler);
    return () => el.removeEventListener("hidden.bs.modal", handler);
  }, [onClose]);

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

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selected = Array.from(e.target.files ?? []);
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

  const handleSubmit = async () => {
    const errors = validateProductForm(form);
    if (errors.length) { toast.error(errors[0]); return; }

    const payload = {
      name: form.name.trim(),
      description: form.description || null,
      price: Number(form.price),
      categoryId: Number(form.categoryId),
      isFeatured: form.isFeatured,
    };

    setIsSubmitting(true);
    let newProductId = null;
    try {
      const res = await createProduct(payload);
      newProductId = res.data.id;
    } catch {
      toast.error("Failed to create product.");
      setIsSubmitting(false);
      return;
    }
    setIsSubmitting(false);

    toast.success("Product created successfully.");
    onSuccess();
    onClose();

    if (files.length > 0 && newProductId) {
      const filesToUpload = [...files];
      const formData = new FormData();
      filesToUpload.forEach((f) => formData.append("files", f));
      uploadProductImage(newProductId, formData)
        .then(() => {
          toast.success("Images uploaded successfully.");
          onSuccessRef.current?.();
        })
        .catch(() => toast.error("Product created, but images could not be uploaded."));
    }
  };

  const imageSection = (
    <div className="mb-3">
      <label className="form-label">Images</label>
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
                style={{ width: 72, height: 72, objectFit: "cover", borderRadius: 6 }}
              />
              <button
                type="button"
                className="btn btn-danger btn-sm"
                style={{ position: "absolute", top: 2, right: 2, padding: "1px 4px", lineHeight: 1 }}
                onClick={() => removeFile(i)}
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
    <div className="modal fade" id="createProductModal" ref={modalRef} tabIndex={-1} aria-hidden="true">
      <div className="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">Create Product</h5>
            <button type="button" className="btn-close" data-bs-dismiss="modal" />
          </div>

          <ProductForm form={form} handleChange={handleChange} imageSection={imageSection} />

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
