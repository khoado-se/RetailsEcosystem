const PAGE_SIZE_OPTIONS = [10, 20, 50];

export default function PageSizeSelector({ pageSize, onPageSizeChange }) {
  return (
    <div className="d-flex align-items-center gap-2">
      <label className="text-muted small mb-0 text-nowrap">Rows per page</label>
      <select
        className="form-select form-select-sm select-auto"
        value={pageSize}
        onChange={(e) => onPageSizeChange(Number(e.target.value))}
      >
        {PAGE_SIZE_OPTIONS.map((size) => (
          <option key={size} value={size}>{size}</option>
        ))}
      </select>
    </div>
  );
}
