import { useProducts } from "../../features/product/useProducts.js";
import { Link, Outlet } from "react-router-dom";
import { useState } from "react";
import Pagination from "./Pagination.jsx";
import CreateProductModal from "../../features/product/CreateProductModal.jsx";
import ProductTable from "../../features/product/ProductTable.jsx";

export default function ProductsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const { products, totalPage, fetchProducts } = useProducts(pageNumber);

  return (
    <>
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
