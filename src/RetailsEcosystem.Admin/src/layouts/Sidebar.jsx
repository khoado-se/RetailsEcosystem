import { NavLink } from "react-router-dom";

export default function Sidebar() {
  return (
    <nav
      id="sidebarMenu"
      className="col-md-3 col-lg-2 d-md-block bg-light sidebar collapse"
    >
      <div className="position-sticky pt-3" bis_skin_checked="1">
        <ul className="nav flex-column">
          <li className="nav-item">
            <NavLink
              className={({ isActive }) =>
                "nav-link " + (isActive ? "active" : "")
              }
              aria-current="page"
              to={"/"}
            >
              <i className="bi bi-house me-2"></i>
              Dashboard
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink
              className={({ isActive }) =>
                "nav-link " + (isActive ? "active" : "")
              }
              to={"/orders"}
            >
              <i className="bi bi-file-earmark me-2"></i>
              Orders
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/products"}>
              <i className="bi bi-cart me-2"></i>
              Products
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/categories"}>
              <i className="bi bi-tags me-2"></i>
              Categories
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/customers"}>
              <i className="bi bi-people me-2"></i>
              Customers
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/reports"}>
              <i className="bi bi-bar-chart me-2"></i>
              Reports
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/integrations"}>
              <i className="bi bi-layers me-2"></i>
              Integrations
            </NavLink>
          </li>
        </ul>

        <h6 className="sidebar-heading d-flex justify-content-between align-items-center px-3 mt-4 mb-1 text-muted">
          <span>Saved reports</span>
          <a className="link-secondary" aria-label="Add a new report">
            <i className="bi bi-plus-circle"></i>
          </a>
        </h6>
        <ul className="nav flex-column mb-2">
          <li className="nav-item">
            <NavLink className="nav-link" to={"/currentMonth"}>
              <i className="bi bi-file-earmark-text me-2"></i>
              Current month
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/lastQuarter"}>
              <i className="bi bi-file-earmark-text me-2"></i>
              Last quarter
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/socialEngagement"}>
              <i className="bi bi-file-earmark-text me-2"></i>
              Social engagement
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink className="nav-link" to={"/yearEndSale"}>
              <i className="bi bi-file-earmark-text me-2"></i>
              Year-end sale
            </NavLink>
          </li>
        </ul>
      </div>
    </nav>
  );
}
