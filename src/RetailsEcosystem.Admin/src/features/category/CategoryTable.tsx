export default function CategoryTable({ categories, onEdit, onDelete }) {
  return (
    <div className="card border-0 shadow-sm">
      <div className="card-body p-0">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
          <thead>
            <tr>
              <th>Name</th>
              <th className="text-end">Actions</th>
            </tr>
          </thead>
          <tbody>
            {categories.length === 0 ? (
              <tr>
                <td colSpan={2} className="text-center text-muted py-4">
                  No categories found.
                </td>
              </tr>
            ) : (
              categories.map((cat) => (
                <tr key={cat.id}>
                  <td className="fw-semibold">{cat.name}</td>
                  <td className="text-end">
                    <button
                      className="btn btn-primary btn-sm me-2"
                      onClick={() => onEdit(cat)}
                    >
                      <i className="bi bi-pencil me-1" />
                      Edit
                    </button>
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => onDelete(cat.id)}
                    >
                      <i className="bi bi-trash me-1" />
                      Delete
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
        </div>
      </div>
    </div>
  );
}
