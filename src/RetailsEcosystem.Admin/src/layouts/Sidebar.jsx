import { useRef, useEffect } from "react";
import { Collapse } from "bootstrap";
import { NavLink, useLocation } from "react-router-dom";

export default function Sidebar({ isOpen, onClose }) {
  const sidebarRef = useRef(null);
  const location = useLocation();

  // Show/hide driven by isOpen prop — single source of truth
  useEffect(() => {
    if (!sidebarRef.current) return;
    const instance = Collapse.getOrCreateInstance(sidebarRef.current, { toggle: false });
    if (isOpen) {
      instance.show();
    } else {
      instance.hide();
    }
  }, [isOpen]);

  // Auto-close sidebar when navigating on mobile
  useEffect(() => {
    onClose();
  }, [location.pathname]);

  return (
    <nav
      id="sidebarMenu"
      ref={sidebarRef}
      className="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse"
    >
      <div className="position-sticky pt-3">
        <ul className="nav flex-column">
          <li className="nav-item">
            <NavLink
              className={({ isActive }) => "nav-link " + (isActive ? "active" : "")}
              aria-current="page"
              to={"/"}
              end
            >
              <i className="bi bi-bar-chart-line me-2"></i>
              Dashboard
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink
              className={({ isActive }) => "nav-link " + (isActive ? "active" : "")}
              to={"/orders"}
            >
              <i className="bi bi-file-earmark me-2"></i>
              Orders
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink
              className={({ isActive }) => "nav-link " + (isActive ? "active" : "")}
              to={"/products"}
            >
              <i className="bi bi-box-seam me-2"></i>
              Products
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink
              className={({ isActive }) => "nav-link " + (isActive ? "active" : "")}
              to={"/categories"}
            >
              <i className="bi bi-tag me-2"></i>
              Categories
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink
              className={({ isActive }) => "nav-link " + (isActive ? "active" : "")}
              to={"/customers"}
            >
              <i className="bi bi-people me-2"></i>
              Customers
            </NavLink>
          </li>
        </ul>
      </div>
    </nav>
  );
}
