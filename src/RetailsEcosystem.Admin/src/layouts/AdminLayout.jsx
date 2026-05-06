import { useState } from "react";
import { Outlet } from "react-router";
import { Toaster } from "react-hot-toast";
import "../pages/dashboard/DashboardPage.css";
import Header from "./Header";
import Footer from "./Footer";
import Sidebar from "./Sidebar";
import Main from "./Main";

export default function AdminLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <>
      <Header onToggleSidebar={() => setSidebarOpen((o) => !o)} />
      <div className="container-fluid">
        <div className="row">
          <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />
          <Main>
            <Outlet />
          </Main>
        </div>
      </div>
      {/* <Footer /> */}
      <Toaster
        position="top-right"
        toastOptions={{
          duration: 3000,
          className: "toast-base",
          success: {
            className: "toast-base toast-success",
            iconTheme: { primary: "#fff", secondary: "#2E9E6B" },
          },
          error: {
            className: "toast-base toast-error",
            iconTheme: { primary: "#fff", secondary: "#D94F4F" },
          },
        }}
      />
    </>
  );
}
