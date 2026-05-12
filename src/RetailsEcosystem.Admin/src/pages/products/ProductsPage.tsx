import { useProducts } from "../../features/product/useProducts";
import { useState, useEffect } from "react";
import CategorySelect from "../../features/category/CategorySelect";
import PageSizeSelector from "../../components/ui/PageSizeSelector";
import CreateProductModal from "../../features/product/CreateProductModal";
import EditProductModal from "../../features/product/EditProductModal";
import ProductTable from "../../features/product/ProductTable";
import ProductImageModal from "../../features/productImage/ProductImageModal";

export default function ProductsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingProductId, setEditingProductId] = useState(null);
  const [imageProduct, setImageProduct] = useState(null);

  const [searchInput, setSearchInput] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [selectedCategoryId, setSelectedCategoryId] = useState("");
  const [featuredOnly, setFeaturedOnly] = useState(false);
  const [sortBy, setSortBy] = useState("createdDate");
  const [sortDesc, setSortDesc] = useState(true);

  const { products, totalPage, loading, error, fetchProducts } = useProducts(
    pageNumber, selectedCategoryId || undefined, debouncedSearch || undefined, pageSize,
    featuredOnly || undefined, sortBy, sortDesc
  );

  // Debounce search input â€” waits 300ms after last keystroke
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchInput);
      setPageNumber(1);
    }, 300);
    return () => clearTimeout(timer);
  }, [searchInput]);

  const handleClearFilters = () => {
    setSearchInput("");
    setDebouncedSearch("");
    setSelectedCategoryId("");
    setFeaturedOnly(false);
    setSortBy("createdDate");
    setSortDesc(true);
    setPageNumber(1);
  };

  const handleSort = (column) => {
    if (sortBy === column) {
      setSortDesc((d) => !d);
    } else {
      setSortBy(column);
      setSortDesc(true);
    }
    setPageNumber(1);
  };

  const handlePageSizeChange = (size) => {
    setPageSize(size);
    setPageNumber(1);
  };

  const hasFilters = searchInput || selectedCategoryId || featuredOnly;

  return (
    <>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h1 className="page-header-title mb-0">Products</h1>
          <p className="page-header-subtitle mb-0">Manage your product catalog</p>
        </div>
        <button
          className="btn btn-primary"
          onClick={() => setIsCreateOpen(true)}
        >
          <i className="bi bi-plus-lg me-1" />
          Create Product
        </button>
      </div>

      {/* Search + Filter Bar */}
      <div className="card border-0 shadow-sm mb-4">
        <div className="card-body py-3">
          <div className="row g-2 align-items-center">
            <div className="col-12 col-md-4">
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
                {searchInput && (
                  <button
                    type="button"
                    className="btn btn-outline-secondary border-start-0"
                    onClick={() => setSearchInput("")}
                    aria-label="Clear search"
                  >
                    <i className="bi bi-x-lg" />
                  </button>
                )}
              </div>
            </div>
            <div className="col-12 col-md-3">
              <CategorySelect
                value={selectedCategoryId}
                onChange={(id) => { setSelectedCategoryId(id); setPageNumber(1); }}
                initialCategoryName=""
              />
            </div>
            <div className="col-12 col-md-2">
              <button
                className={`btn w-100 ${featuredOnly ? "btn-primary" : "btn-outline-secondary"}`}
                onClick={() => { setFeaturedOnly((f) => !f); setPageNumber(1); }}
              >
                <i className="bi bi-star-fill me-1" />
                Featured
              </button>
            </div>
            <div className="col-12 col-md-1">
              {hasFilters && (
                <button
                  className="btn btn-outline-secondary w-100"
                  onClick={handleClearFilters}
                >
                  <i className="bi bi-x-lg me-1" />
                  Clear
                </button>
              )}
            </div>
            <div className="col-12 col-md-2 d-flex justify-content-end">
              <PageSizeSelector pageSize={pageSize} onPageSizeChange={handlePageSizeChange} />
            </div>
          </div>
        </div>
      </div>

      {error && (
        <div className="alert alert-danger d-flex align-items-start gap-2 rounded-3 mb-3" role="alert">
          <i className="bi bi-exclamation-circle-fill flex-shrink-0 mt-1" />
          <div>
            {error}
            <button className="btn btn-sm btn-link p-0 ms-2" onClick={fetchProducts}>
              Retry
            </button>
          </div>
        </div>
      )}

      <ProductTable
        products={products}
        pageNumber={pageNumber}
        setPageNumber={setPageNumber}
        totalPage={totalPage}
        loading={loading}
        onEdit={setEditingProductId}
        onDeleted={() => { fetchProducts(); setPageNumber(1); }}
        onImages={(id, name) => setImageProduct({ id, name })}
        onClearFilters={hasFilters ? handleClearFilters : undefined}
        sortBy={sortBy}
        sortDesc={sortDesc}
        onSort={handleSort}
      />

      <CreateProductModal
        isOpen={isCreateOpen}
        onSuccess={() => {
          setPageNumber(1);
          fetchProducts();
        }}
        onClose={() => setIsCreateOpen(false)}
      />

      <EditProductModal
        productId={editingProductId}
        onSuccess={() => fetchProducts()}
        onClose={() => setEditingProductId(null)}
      />

      <ProductImageModal
        productId={imageProduct?.id}
        productName={imageProduct?.name}
        onClose={() => setImageProduct(null)}
        onChanged={() => fetchProducts()}
      />
    </>
  );
}
