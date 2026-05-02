import { useProducts } from "../../features/product/useProducts.js";
import { useCategories } from "../../features/category/useCategories.js";
import { useState, useEffect } from "react";
import CreateProductModal from "../../features/product/CreateProductModal.jsx";
import EditProductModal from "../../features/product/EditProductModal.jsx";
import ProductTable from "../../features/product/ProductTable.jsx";
import ProductImageModal from "../../features/productImage/ProductImageModal.jsx";

export default function ProductsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const [editingProductId, setEditingProductId] = useState(null);
  const [imageProduct, setImageProduct] = useState(null);

  const [searchInput, setSearchInput] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [selectedCategoryId, setSelectedCategoryId] = useState("");

  const { categories } = useCategories();
  const { products, totalPage, fetchProducts } = useProducts(pageNumber, selectedCategoryId || undefined, debouncedSearch || undefined);

  // Debounce search input — waits 300ms after last keystroke
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchInput);
      setPageNumber(1);
    }, 300);
    return () => clearTimeout(timer);
  }, [searchInput]);

  const handleCategoryChange = (e) => {
    setSelectedCategoryId(e.target.value);
    setPageNumber(1);
  };

  const handleClearFilters = () => {
    setSearchInput("");
    setDebouncedSearch("");
    setSelectedCategoryId("");
    setPageNumber(1);
  };

  const hasFilters = searchInput || selectedCategoryId;

  return (
    <>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h1 className="page-header-title mb-0">Products</h1>
          <p className="page-header-subtitle mb-0">Manage your product catalog</p>
        </div>
        <button
          className="btn btn-primary"
          data-bs-toggle="modal"
          data-bs-target="#createProductModal"
        >
          <i className="bi bi-plus-lg me-1" />
          Create Product
        </button>
      </div>

      {/* Search + Filter Bar */}
      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body py-3">
          <div className="row g-2 align-items-center">
            <div className="col-12 col-md-5">
              <div className="input-group">
                <span className="input-group-text bg-white border-end-0">
                  <i className="bi bi-search text-muted" />
                </span>
                <input
                  type="text"
                  className="form-control border-start-0"
                  placeholder="Search products by name..."
                  value={searchInput}
                  onChange={(e) => setSearchInput(e.target.value)}
                />
              </div>
            </div>
            <div className="col-12 col-md-4">
              <select
                className="form-select"
                value={selectedCategoryId}
                onChange={handleCategoryChange}
              >
                <option value="">All Categories</option>
                {categories.map((cat) => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
            </div>
            <div className="col-12 col-md-3">
              {hasFilters && (
                <button
                  className="btn btn-outline-secondary w-100"
                  onClick={handleClearFilters}
                >
                  <i className="bi bi-x-lg me-1" />
                  Clear Filters
                </button>
              )}
            </div>
          </div>
        </div>
      </div>

      <ProductTable
        products={products}
        pageNumber={pageNumber}
        setPageNumber={setPageNumber}
        totalPage={totalPage}
        onEdit={setEditingProductId}
        onDeleted={() => { fetchProducts(); setPageNumber(1); }}
        onImages={(id, name) => setImageProduct({ id, name })}
      />

      <CreateProductModal
        onSuccess={() => {
          fetchProducts();
          setPageNumber(1);
        }}
      />

      <EditProductModal
        productId={editingProductId}
        onSuccess={() => fetchProducts()}
      />

      <ProductImageModal
        productId={imageProduct?.id}
        productName={imageProduct?.name}
        onClose={() => setImageProduct(null)}
      />
    </>
  );
}
