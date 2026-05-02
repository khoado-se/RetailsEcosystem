import { useState, useContext } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { AuthContext } from "../../contexts/AuthContext";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const { login } = useContext(AuthContext);
  const navigate = useNavigate();
  const location = useLocation();

  const from = location.state?.from?.pathname || "/";

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      await login({ email, password });
      navigate(from, { replace: true });
    } catch (err) {
      setError(
        err.response?.data?.errors?.join(", ") ||
          err.response?.data?.title ||
          "Failed to login. Please check your credentials."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="brand-panel-bg min-vh-100 d-flex align-items-stretch">
      {/* Left branding panel */}
      <div
        className="d-none d-lg-flex flex-column align-items-center justify-content-center text-white p-5"
        style={{ width: "45%" }}
      >
        <div className="text-center">
          <div className="brand-logo-badge rounded-circle d-inline-flex align-items-center justify-content-center mb-4 overflow-hidden">
            <img src="/brand/logo-icon.svg" alt="RetailsEcosystem" style={{ width: "60%", height: "60%" }} />
          </div>
          <div className="mb-2">
            <img src="/brand/logo-full-dark.svg" alt="RetailsEcosystem" style={{ height: "36px" }} />
          </div>
          <p className="mb-5 text-white-50 text-body-sm">
            Unified commerce management platform
          </p>

          <div className="d-flex flex-column gap-3 text-start">
            {[
              { icon: "bi-grid-1x2-fill", label: "Centralized product catalog" },
              { icon: "bi-people-fill", label: "Customer & order management" },
              { icon: "bi-bar-chart-line-fill", label: "Real-time analytics dashboard" },
            ].map(({ icon, label }) => (
              <div key={label} className="d-flex align-items-center gap-3">
                <div
                  className="bg-primary-tint rounded-3 d-flex align-items-center justify-content-center flex-shrink-0"
                  style={{ width: "38px", height: "38px" }}
                >
                  <i className={`bi ${icon} text-primary-brand text-caption`} />
                </div>
                <span className="text-white-50 text-body-sm">{label}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Right form panel */}
      <div className="flex-grow-1 d-flex align-items-center justify-content-center p-4">
        <div style={{ width: "100%", maxWidth: "440px" }}>
          {/* Mobile logo */}
          <div className="text-center mb-4 d-lg-none">
            <div className="brand-logo-badge-sm rounded-circle d-inline-flex align-items-center justify-content-center mb-2 overflow-hidden">
              <img src="/brand/logo-icon.svg" alt="RetailsEcosystem" style={{ width: "60%", height: "60%" }} />
            </div>
            <div className="fw-bold text-white" style={{ fontSize: "1.1rem" }}>
              RetailsEcosystem
            </div>
          </div>

          <div className="card-panel card border-0 shadow-sm">
            <div className="card-body p-4 p-md-5">
              <div className="mb-4">
                <h2 className="text-page-title mb-1">Welcome back</h2>
                <p className="text-muted mb-0 text-body-sm">
                  Sign in to your admin account
                </p>
              </div>

              <form onSubmit={handleSubmit} noValidate>
                {error && (
                  <div className="alert alert-danger d-flex align-items-start gap-2 rounded-3 py-2 px-3 mb-4" role="alert">
                    <i className="bi bi-exclamation-circle-fill flex-shrink-0 mt-1 text-caption" />
                    <span className="text-body-sm">{error}</span>
                  </div>
                )}

                {/* Email field */}
                <div className="mb-3">
                  <label htmlFor="email-address" className="form-label text-label">
                    Email address
                  </label>
                  <div className="input-group">
                    <span className="input-group-text bg-light border-end-0">
                      <i className="bi bi-envelope text-muted" />
                    </span>
                    <input
                      id="email-address"
                      name="email"
                      type="email"
                      className="form-control form-control-lg border-start-0 bg-light"
                      placeholder="you@example.com"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      required
                      autoComplete="email"
                    />
                  </div>
                </div>

                {/* Password field */}
                <div className="mb-3">
                  <label htmlFor="password" className="form-label text-label">
                    Password
                  </label>
                  <div className="input-group">
                    <span className="input-group-text bg-light border-end-0">
                      <i className="bi bi-lock text-muted" />
                    </span>
                    <input
                      id="password"
                      name="password"
                      type={showPassword ? "text" : "password"}
                      className="form-control form-control-lg border-start-0 border-end-0 bg-light"
                      placeholder="Enter your password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      required
                      autoComplete="current-password"
                    />
                    <button
                      type="button"
                      className="input-group-text bg-light border-start-0"
                      style={{ cursor: "pointer" }}
                      onClick={() => setShowPassword((v) => !v)}
                      tabIndex={-1}
                      aria-label={showPassword ? "Hide password" : "Show password"}
                    >
                      <i className={`bi ${showPassword ? "bi-eye-slash" : "bi-eye"} text-muted`} />
                    </button>
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={loading}
                  className="btn btn-primary btn-lg w-100 fw-semibold"
                >
                  {loading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true" />
                      Signing in…
                    </>
                  ) : (
                    <>
                      Sign in
                      <i className="bi bi-arrow-right ms-2" />
                    </>
                  )}
                </button>
              </form>

              <div className="text-center mt-4">
                <small className="text-muted text-caption">
                  <i className="bi bi-shield-lock me-1" />
                  Secure admin access · RetailsEcosystem
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
