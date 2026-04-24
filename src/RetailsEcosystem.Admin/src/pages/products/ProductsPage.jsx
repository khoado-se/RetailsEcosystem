import { useProducts } from "../../features/product/useProducts.js";
import { Link, Outlet } from "react-router-dom";
import { useState } from "react";
import Pagination from "./Pagination.jsx";
import CreateProductModal from "./CreateProductModal.jsx";
import ProductTable from "../../features/product/ProductTable.jsx";

export default function ProductsPage() {
  const [page, setPage] = useState(1);
  const { products, totalPage } = useProducts(page);

  return (
    <>
      <ProductTable
        products={products}
        page={page}
        setPage={setPage}
        totalPage={totalPage}
      />
      <CreateProductModal onSuccess={() => setPage(1)} />
    </>
  );
}
