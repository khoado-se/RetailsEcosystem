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
          style: {
            background: "#fff",
            color: "#333",
            borderRadius: "8px",
            boxShadow: "0 4px 12px rgba(0,0,0,0.12)",
          },
          success: {
            iconTheme: { primary: "#4A90D9", secondary: "#fff" },
          },
        }}
      />
    </>
  );
}
