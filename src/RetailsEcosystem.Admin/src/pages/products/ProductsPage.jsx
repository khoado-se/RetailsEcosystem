import { useProducts } from "../../features/product/useProducts.js";
import { useState } from "react";
import CreateProductModal from "../../features/product/CreateProductModal.jsx";
import EditProductModal from "../../features/product/EditProductModal.jsx";
import ProductTable from "../../features/product/ProductTable.jsx";
import ProductImageModal from "../../features/productImage/ProductImageModal.jsx";

export default function ProductsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const [editingProductId, setEditingProductId] = useState(null);
  const [imageProduct, setImageProduct] = useState(null);
  const { products, totalPage, fetchProducts } = useProducts(pageNumber);

  const handleEdited = () => {
    fetchProducts();
  };

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
        onSuccess={handleEdited}
      />

      <ProductImageModal
        productId={imageProduct?.id}
        productName={imageProduct?.name}
        onClose={() => setImageProduct(null)}
      />
    </>
  );
}
