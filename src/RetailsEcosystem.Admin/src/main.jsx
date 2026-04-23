import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import OrdersPage from "./pages/order/OrdersPage.jsx";
import ProductsPage from "./pages/products/ProductsPage.jsx";
import CategoriesPage from "./pages/categories/CategoriesPage.jsx";
import CustomerListPage from "./pages/customers/CustomerListPage.jsx";
import NotFoundPage from "./pages/notfound/NotFoundPage.jsx";
import CreateProductPage from "./pages/products/CreateProductPage.jsx";
import "bootstrap-icons/font/bootstrap-icons.css";

import "./index.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import DashboardPage from "./pages/dashboard/DashboardPage.jsx";
import App from "./App.jsx";

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<App />}>
          <Route index element={<DashboardPage />} />
          <Route path="/orders" element={<OrdersPage />} />
          <Route path="/products" element={<ProductsPage />} >
            <Route path="create" element={<CreateProductPage />} />
          </Route>
          <Route path="/categories" element={<CategoriesPage />} />
          <Route path="/customers" element={<CustomerListPage />} />
          <Route path="**" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </StrictMode>,
);
