import { useContext } from "react";
import { AuthContext } from "../contexts/AuthContext";

export default function Header({ onToggleSidebar }) {
  const { user, logout } = useContext(AuthContext);

  const handleLogout = async (e) => {
    e.preventDefault();
    await logout();
  };

  return (
    <header className="navbar navbar-dark sticky-top bg-brand-nav flex-md-nowrap p-0 shadow">
      <a className="navbar-brand col-md-3 col-lg-2 me-0 px-3" href="/">
        <img src="/brand/logo-full-dark.svg" alt="RetailsEcosystem" style={{ height: "28px" }} />
      </a>

     

      <div className="navbar-nav ms-auto d-flex align-items-center flex-row gap-2 px-3">
        {user && (
          <div className="d-flex align-items-center gap-2">
            <div
              className="rounded-circle bg-primary-tint text-primary-brand d-flex align-items-center justify-content-center fw-bold flex-shrink-0 text-body-sm"
              style={{ width: "32px", height: "32px" }}
            >
              {user?.fullName?.[0]?.toUpperCase() ?? "?"}
            </div>
            <span className="text-white text-body-sm d-none d-md-inline">{user?.fullName}</span>
          </div>
        )}
        <div className="nav-item text-nowrap">
          <button className="btn btn-link text-white p-0 text-body-sm nav-link" onClick={handleLogout}>
            Sign out
          </button>
        </div>
      </div>

       <button
        className="btn btn-link text-white d-md-none px-3"
        type="button"
        onClick={onToggleSidebar}
        aria-label="Toggle navigation"
      >
        <i className="bi bi-list fs-5" />
      </button>
    </header>
  );
}
