import { useEffect, useRef, useState, useCallback } from "react";
import Chart from "chart.js/auto";
import { getOrderStats } from "../../features/order/orderApi";
import "./DashboardPage.css";

export default function DashboardPage() {
  const chartRef = useRef(null);
  const chartInstance = useRef(null);
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchStats = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await getOrderStats();
      setStats(res.data);
    } catch (err) {
      setError(err.response?.data?.title || err.message || "Failed to load stats.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetchStats(); }, [fetchStats]);

  useEffect(() => {
    if (!stats?.dailyRevenue?.length) return;

    if (chartInstance.current) chartInstance.current.destroy();

    chartInstance.current = new Chart(chartRef.current, {
      type: "line",
      data: {
        labels: stats.dailyRevenue.map((d) => d.date),
        datasets: [
          {
            data: stats.dailyRevenue.map((d) => d.revenue),
            tension: 0.3,
            backgroundColor: "transparent",
            borderColor: "#4A90D9",
            borderWidth: 4,
            pointBackgroundColor: "#4A90D9",
          },
        ],
      },
      options: {
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: (v) => `$${Number(v).toLocaleString()}`,
            },
          },
        },
        plugins: { legend: { display: false } },
      },
    });

    return () => chartInstance.current?.destroy();
  }, [stats]);

  const fmt = (val) =>
    val != null
      ? Number(val).toLocaleString("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 })
      : "—";

  return (
    <>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h1 className="page-header-title mb-0">Dashboard</h1>
          <p className="page-header-subtitle mb-0">Overview of sales and activity</p>
        </div>
      </div>

      {loading ? (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Loading…</span>
          </div>
        </div>
      ) : error ? (
        <div className="alert alert-danger d-flex align-items-start gap-2 rounded-3 mb-4" role="alert">
          <i className="bi bi-exclamation-circle-fill flex-shrink-0 mt-1" />
          <div>
            <strong>Could not load stats</strong>
            <p className="mb-1 small">{error}</p>
            <button className="btn btn-sm btn-outline-danger" onClick={fetchStats}>
              Retry
            </button>
          </div>
        </div>
      ) : (
        <>
          {/* KPI Cards */}
          <div className="row g-3 mb-4">
            <div className="col-sm-4">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-body d-flex align-items-center gap-3 p-4">
                  <div className="icon-badge-md bg-primary-tint rounded-3 d-flex align-items-center justify-content-center">
                    <i className="bi bi-bag-check fs-5 text-primary-brand" />
                  </div>
                  <div>
                    <div className="text-muted small">Orders Today</div>
                    <div className="fs-4 fw-bold">{stats ? stats.ordersToday : "—"}</div>
                  </div>
                </div>
              </div>
            </div>

            <div className="col-sm-4">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-body d-flex align-items-center gap-3 p-4">
                  <div className="icon-badge-md bg-success-tint rounded-3 d-flex align-items-center justify-content-center">
                    <i className="bi bi-currency-dollar fs-5 text-success" />
                  </div>
                  <div>
                    <div className="text-muted small">Revenue This Month</div>
                    <div className="fs-4 fw-bold">{stats ? fmt(stats.revenueThisMonth) : "—"}</div>
                  </div>
                </div>
              </div>
            </div>

            <div className="col-sm-4">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-body d-flex align-items-center gap-3 p-4">
                  <div className="icon-badge-md bg-warning-tint rounded-3 d-flex align-items-center justify-content-center">
                    <i className="bi bi-hourglass-split fs-5 text-warning" />
                  </div>
                  <div>
                    <div className="text-muted small">Pending Orders</div>
                    <div className="fs-4 fw-bold">{stats ? stats.pendingOrders : "—"}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-4">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-3">
                <div className="fw-semibold">Revenue — Last 7 Days</div>
              </div>
              {stats?.dailyRevenue?.length ? (
                <canvas ref={chartRef} className="w-100" height={100}></canvas>
              ) : (
                <div className="d-flex justify-content-center align-items-center" style={{ height: 100 }}>
                  <div className="spinner-border spinner-border-sm text-secondary" role="status" />
                </div>
              )}
            </div>
          </div>
        </>
      )}
    </>
  );
}
