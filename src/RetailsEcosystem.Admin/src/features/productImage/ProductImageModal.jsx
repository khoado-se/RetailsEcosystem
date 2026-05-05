import { useEffect, useRef, useState } from "react";
import { Modal } from "bootstrap";
import toast from "react-hot-toast";
import { getProductImages, uploadProductImage, deleteProductImage } from "./productImageApi";

export default function ProductImageModal({ productId, productName, onClose }) {
  const [images, setImages] = useState([]);
  const [files, setFiles] = useState([]);
  const [previews, setPreviews] = useState([]);
  const [uploading, setUploading] = useState(false);
  const [deletingId, setDeletingId] = useState(null);
  const [error, setError] = useState(null);
  const inputRef = useRef(null);
  const modalRef = useRef(null);

  // Show/hide driven by productId — single source of truth, no data-bs-toggle
  useEffect(() => {
    if (!modalRef.current) return;
    const instance = Modal.getOrCreateInstance(modalRef.current);
    if (productId) {
      instance.show();
    } else {
      instance.hide();
    }
  }, [productId]);

  // Sync all Bootstrap close paths (X, Escape, click-outside) back to React
  useEffect(() => {
    const el = modalRef.current;
    if (!el) return;
    const handler = () => onClose?.();
    el.addEventListener("hidden.bs.modal", handler);
    return () => el.removeEventListener("hidden.bs.modal", handler);
  }, [onClose]);

  useEffect(() => {
    if (!productId) return;
    setImages([]);
    setError(null);
    getProductImages(productId)
      .then((res) => setImages(res.data))
      .catch(() => setError("Failed to load images."));
  }, [productId]);

  useEffect(() => {
    const urls = files.map((f) => URL.createObjectURL(f));
    setPreviews(urls);
    return () => urls.forEach((u) => URL.revokeObjectURL(u));
  }, [files]);

  const handleFileChange = (e) => {
    const selected = Array.from(e.target.files);
    const allowed = [".jpg", ".jpeg", ".png", ".webp"];
    const invalid = selected.filter(
      (f) => !allowed.some((ext) => f.name.toLowerCase().endsWith(ext))
    );
    if (invalid.length > 0) {
      setError(`Unsupported file type: ${invalid.map((f) => f.name).join(", ")}`);
      e.target.value = "";
      return;
    }
    setError(null);
    setFiles(selected);
  };

  const handleUpload = async () => {
    if (files.length === 0) return;
    setUploading(true);
    setError(null);
    try {
      const formData = new FormData();
      files.forEach((f) => formData.append("files", f));
      await uploadProductImage(productId, formData);
      const newImages = await getProductImages(productId);
      setImages(newImages.data);
      setFiles([]);
      if (inputRef.current) inputRef.current.value = "";
      toast.success("Image uploaded successfully.");
    } catch {
      setError("Upload failed. Please try again.");
    } finally {
      setUploading(false);
    }
  };

  const handleDelete = async (imageId) => {
    setDeletingId(imageId);
    setError(null);
    try {
      await deleteProductImage(imageId);
      setImages((prev) => prev.filter((img) => img.id !== imageId));
      toast.success("Image deleted.");
    } catch {
      setError("Delete failed. Please try again.");
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div
      className="modal fade"
      id="productImageModal"
      ref={modalRef}
      tabIndex="-1"
      aria-labelledby="productImageModalLabel"
      aria-hidden="true"
    >
      <div className="modal-dialog modal-lg modal-dialog-scrollable">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title" id="productImageModalLabel">
              Images — {productName}
            </h5>
            <button
              type="button"
              className="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            />
          </div>

          <div className="modal-body">
            {error && (
              <div className="alert alert-danger py-2 mb-3">{error}</div>
            )}

            {/* Image grid */}
            <div className="d-flex flex-wrap gap-2 mb-4">
              {images.length === 0 && !error && (
                <p className="text-muted small">No images yet.</p>
              )}
              {images.map((img) => (
                <div
                  key={img.id}
                  style={{ position: "relative", width: 100, height: 100 }}
                >
                  <img
                    src={img.url}
                    alt=""
                    style={{
                      width: 100,
                      height: 100,
                      objectFit: "cover",
                      borderRadius: 6,
                    }}
                  />
                  <button
                    className="btn btn-danger btn-sm"
                    style={{
                      position: "absolute",
                      top: 4,
                      right: 4,
                      padding: "2px 5px",
                      lineHeight: 1,
                    }}
                    onClick={() => handleDelete(img.id)}
                    disabled={deletingId === img.id}
                    title="Delete image"
                  >
                    {deletingId === img.id ? (
                      <span
                        className="spinner-border spinner-border-sm"
                        role="status"
                      />
                    ) : (
                      <i className="bi bi-trash" />
                    )}
                  </button>
                </div>
              ))}
            </div>

            {/* Upload zone */}
            <div
              className="border border-dashed rounded p-3"
              style={{ borderStyle: "dashed" }}
            >
              <p className="text-muted small mb-2">
                Select images to upload (.jpg, .jpeg, .png, .webp)
              </p>
              <input
                ref={inputRef}
                type="file"
                className="form-control mb-2"
                multiple
                accept=".jpg,.jpeg,.png,.webp"
                onChange={handleFileChange}
              />

              {previews.length > 0 && (
                <div className="d-flex flex-wrap gap-2 mb-2">
                  {previews.map((src, i) => (
                    <img
                      key={i}
                      src={src}
                      alt=""
                      style={{
                        width: 80,
                        height: 80,
                        objectFit: "cover",
                        borderRadius: 4,
                        opacity: 0.85,
                      }}
                    />
                  ))}
                </div>
              )}

              <button
                className="btn btn-primary btn-sm"
                onClick={handleUpload}
                disabled={files.length === 0 || uploading}
              >
                {uploading ? (
                  <>
                    <span
                      className="spinner-border spinner-border-sm me-1"
                      role="status"
                    />
                    Uploading…
                  </>
                ) : (
                  <>
                    <i className="bi bi-cloud-upload me-1" />
                    Upload {files.length > 0 ? `${files.length} file${files.length > 1 ? "s" : ""}` : ""}
                  </>
                )}
              </button>
            </div>
          </div>

          <div className="modal-footer">
            <button
              type="button"
              className="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
