import { useEffect, useRef, useState } from "react";
import Chart from "chart.js/auto";
import { getOrderStats } from "../../features/order/orderApi";
import "./DashboardPage.css";

export default function DashboardPage() {
  const chartRef = useRef(null);
  const chartInstance = useRef(null);
  const [stats, setStats] = useState(null);

  useEffect(() => {
    getOrderStats()
      .then((res) => setStats(res.data))
      .catch(() => setStats(null));
  }, []);

  useEffect(() => {
    const ctx = chartRef.current;

    chartInstance.current = new Chart(ctx, {
      type: "line",
      data: {
        labels: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
        datasets: [
          {
            data: [15339, 21345, 18483, 24003, 23489, 24092, 12034],
            tension: 0,
            backgroundColor: "transparent",
            borderColor: "#4A90D9",
            borderWidth: 4,
            pointBackgroundColor: "#4A90D9",
          },
        ],
      },
      options: {
        scales: { y: { beginAtZero: false } },
        plugins: { legend: { display: false } },
      },
    });

    return () => {
      chartInstance.current.destroy();
    };
  }, []);

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

      {/* KPI Cards */}
      <div className="row g-3 mb-4">
        <div className="col-sm-4">
          <div className="card border-0 shadow-sm h-100">
            <div className="card-body d-flex align-items-center gap-3 p-4">
              <div className="rounded-3 d-flex align-items-center justify-content-center"
                style={{ width: 48, height: 48, background: "rgba(74,144,217,0.12)" }}>
                <i className="bi bi-bag-check fs-5" style={{ color: "#4A90D9" }} />
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
              <div className="rounded-3 d-flex align-items-center justify-content-center"
                style={{ width: 48, height: 48, background: "rgba(25,135,84,0.12)" }}>
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
              <div className="rounded-3 d-flex align-items-center justify-content-center"
                style={{ width: 48, height: 48, background: "rgba(255,193,7,0.15)" }}>
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
          <canvas ref={chartRef} className="w-100" height={100}></canvas>
        </div>
      </div>
    </>
  );
}
