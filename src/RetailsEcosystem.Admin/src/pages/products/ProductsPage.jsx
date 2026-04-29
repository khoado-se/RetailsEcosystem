import { useProducts } from "../../features/product/useProducts.js";
import { useState } from "react";
import CreateProductModal from "../../features/product/CreateProductModal.jsx";
import ProductTable from "../../features/product/ProductTable.jsx";

export default function ProductsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const { products, totalPage, fetchProducts } = useProducts(pageNumber);

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

      <ProductTable
        products={products}
        pageNumber={pageNumber}
        setPageNumber={setPageNumber}
        totalPage={totalPage}
      />
      <CreateProductModal
        onSuccess={() => {
          fetchProducts();
          setPageNumber(1);
        }}
      />
    </>
  );
}
