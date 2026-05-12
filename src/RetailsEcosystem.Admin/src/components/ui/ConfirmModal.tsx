import { useEffect, useRef } from "react";
import { Modal } from "bootstrap";

export default function ConfirmModal({ title, message, confirmLabel = "Delete", onConfirm, onClose }) {
  const modalRef = useRef(null);

  useEffect(() => {
    if (!modalRef.current) return;
    const instance = Modal.getOrCreateInstance(modalRef.current, { backdrop: "static" });
    instance.show();

    const el = modalRef.current;
    const handleHidden = () => onClose();
    el.addEventListener("hidden.bs.modal", handleHidden);
    return () => {
      el.removeEventListener("hidden.bs.modal", handleHidden);
      instance.hide();
    };
  }, []);

  const handleConfirm = () => {
    Modal.getOrCreateInstance(modalRef.current).hide();
    onConfirm();
  };

  return (
    <div className="modal fade" ref={modalRef} tabIndex={-1} aria-hidden="true">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content rounded-3 shadow">
          <div className="modal-header border-0 pb-0">
            <h5 className="modal-title fw-bold">{title}</h5>
            <button type="button" className="btn-close" onClick={() => Modal.getOrCreateInstance(modalRef.current).hide()} aria-label="Close" />
          </div>
          <div className="modal-body pt-2">{message}</div>
          <div className="modal-footer border-0 pt-0">
            <button type="button" className="btn btn-outline-secondary rounded-pill px-4"
              onClick={() => Modal.getOrCreateInstance(modalRef.current).hide()}>
              Cancel
            </button>
            <button type="button" className="btn btn-danger rounded-pill px-4" onClick={handleConfirm}>
              {confirmLabel}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
