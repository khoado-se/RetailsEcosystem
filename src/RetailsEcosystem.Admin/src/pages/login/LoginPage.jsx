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
    <div
      className="min-vh-100 d-flex align-items-stretch"
      style={{ background: "linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)" }}
    >
      {/* Left branding panel */}
      <div
        className="d-none d-lg-flex flex-column align-items-center justify-content-center text-white p-5"
        style={{ width: "45%", background: "rgba(255,255,255,0.04)" }}
      >
        <div className="text-center">
          <div
            className="rounded-circle d-inline-flex align-items-center justify-content-center mb-4"
            style={{
              width: "90px",
              height: "90px",
              background: "linear-gradient(135deg, #e94560 0%, #f5a623 100%)",
              fontSize: "36px",
              fontWeight: 700,
              boxShadow: "0 8px 32px rgba(233,69,96,0.4)",
            }}
          >
            R
          </div>
          <h1 className="fw-bold mb-2" style={{ fontSize: "2rem", letterSpacing: "-0.5px" }}>
            RetailsEcosystem
          </h1>
          <p className="mb-5" style={{ color: "rgba(255,255,255,0.6)", fontSize: "1rem" }}>
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
                  className="rounded-3 d-flex align-items-center justify-content-center flex-shrink-0"
                  style={{
                    width: "38px",
                    height: "38px",
                    background: "rgba(255,255,255,0.1)",
                  }}
                >
                  <i className={`bi ${icon}`} style={{ color: "#e94560", fontSize: "16px" }} />
                </div>
                <span style={{ color: "rgba(255,255,255,0.8)", fontSize: "0.9rem" }}>{label}</span>
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
            <div
              className="rounded-circle d-inline-flex align-items-center justify-content-center mb-2"
              style={{
                width: "60px",
                height: "60px",
                background: "linear-gradient(135deg, #e94560 0%, #f5a623 100%)",
                fontSize: "24px",
                fontWeight: 700,
              }}
            >
              R
            </div>
            <div className="fw-bold text-white" style={{ fontSize: "1.1rem" }}>
              RetailsEcosystem
            </div>
          </div>

          <div className="card border-0 shadow-sm" style={{ borderRadius: "16px" }}>
            <div className="card-body p-4 p-md-5">
              <div className="mb-4">
                <h2 className="fw-bold mb-1" style={{ fontSize: "1.6rem", color: "#1a1a2e" }}>
                  Welcome back
                </h2>
                <p className="text-muted mb-0" style={{ fontSize: "0.9rem" }}>
                  Sign in to your admin account
                </p>
              </div>

              <form onSubmit={handleSubmit} noValidate>
                {error && (
                  <div className="alert alert-danger d-flex align-items-start gap-2 rounded-3 py-2 px-3 mb-4" role="alert">
                    <i className="bi bi-exclamation-circle-fill flex-shrink-0 mt-1" style={{ fontSize: "14px" }} />
                    <span style={{ fontSize: "0.875rem" }}>{error}</span>
                  </div>
                )}

                {/* Email field */}
                <div className="mb-3">
                  <label htmlFor="email-address" className="form-label fw-semibold" style={{ fontSize: "0.875rem" }}>
                    Email address
                  </label>
                  <div className="input-group">
                    <span className="input-group-text bg-light border-end-0" style={{ borderRadius: "10px 0 0 10px" }}>
                      <i className="bi bi-envelope text-muted" />
                    </span>
                    <input
                      id="email-address"
                      name="email"
                      type="email"
                      className="form-control border-start-0 bg-light"
                      style={{ borderRadius: "0 10px 10px 0" }}
                      placeholder="you@example.com"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      required
                      autoComplete="email"
                    />
                  </div>
                </div>

                {/* Password field */}
                <div className="mb-4">
                  <label htmlFor="password" className="form-label fw-semibold" style={{ fontSize: "0.875rem" }}>
                    Password
                  </label>
                  <div className="input-group">
                    <span className="input-group-text bg-light border-end-0" style={{ borderRadius: "10px 0 0 10px" }}>
                      <i className="bi bi-lock text-muted" />
                    </span>
                    <input
                      id="password"
                      name="password"
                      type={showPassword ? "text" : "password"}
                      className="form-control border-start-0 border-end-0 bg-light"
                      placeholder="Enter your password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      required
                      autoComplete="current-password"
                    />
                    <button
                      type="button"
                      className="input-group-text bg-light border-start-0"
                      style={{ borderRadius: "0 10px 10px 0", cursor: "pointer" }}
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
                  className="btn btn-lg w-100 text-white fw-semibold"
                  style={{
                    background: "linear-gradient(135deg, #e94560 0%, #c73652 100%)",
                    border: "none",
                    borderRadius: "10px",
                    boxShadow: loading ? "none" : "0 4px 15px rgba(233,69,96,0.35)",
                    transition: "all 0.2s",
                  }}
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
                <small className="text-muted" style={{ fontSize: "0.8rem" }}>
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
