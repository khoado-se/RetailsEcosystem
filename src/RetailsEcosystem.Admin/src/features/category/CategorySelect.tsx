import { useEffect, useRef, useState, useCallback } from "react";
import { useCategoriesInfinite } from "./useCategories";

export default function CategorySelect({ value, onChange, initialCategoryName }) {
  const [open, setOpen] = useState(false);
  const containerRef = useRef(null);
  const sentinelRef = useRef(null);
  const { items, hasMore, loading, loadMore, reset } = useCategoriesInfinite(10);

  const openDropdown = useCallback(() => {
    reset();
    setOpen(true);
  }, [reset]);

  // Load page 1 when dropdown opens (after reset clears state)
  useEffect(() => {
    if (open && items.length === 0 && !loading) {
      loadMore();
    }
  }, [open, items.length, loading, loadMore]);

  // Close on outside click
  useEffect(() => {
    if (!open) return;
    const handleMouseDown = (e) => {
      if (containerRef.current && !containerRef.current.contains(e.target)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handleMouseDown);
    return () => document.removeEventListener("mousedown", handleMouseDown);
  }, [open]);

  // IntersectionObserver on sentinel to trigger loadMore
  useEffect(() => {
    if (!open || !sentinelRef.current) return;
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting && hasMore && !loading) {
          loadMore();
        }
      },
      { threshold: 0.1 }
    );
    observer.observe(sentinelRef.current);
    return () => observer.disconnect();
  }, [open, hasMore, loading, loadMore]);

  const selectedName =
    items.find((c) => String(c.id) === String(value))?.name ??
    initialCategoryName ??
    "-- Select --";

  const triggerLabel = value ? selectedName : "-- Select --";

  const handleSelect = (id) => {
    onChange(id);
    setOpen(false);
  };

  return (
    <div ref={containerRef} className="cs-admin-wrapper">
      <button
        type="button"
        className="form-select text-start cs-admin-trigger"
        onClick={() => (open ? setOpen(false) : openDropdown())}
      >
        {triggerLabel}
      </button>

      {open && (
        <div className="cs-admin-panel border rounded bg-white shadow-sm">
          <div
            className="category-option"
            onClick={() => handleSelect("")}
          >
            -- Select --
          </div>

          {items.map((c) => (
            <div
              key={c.id}
              className={`category-option${String(c.id) === String(value) ? " category-option--selected" : ""}`}
              onClick={() => handleSelect(c.id)}
            >
              {c.name}
            </div>
          ))}

          <div ref={sentinelRef} className="cs-admin-sentinel" />

          {loading && (
            <div className="text-center py-2">
              <span className="spinner-border spinner-border-sm text-primary" role="status" aria-hidden="true" />
            </div>
          )}
        </div>
      )}
    </div>
  );
}
