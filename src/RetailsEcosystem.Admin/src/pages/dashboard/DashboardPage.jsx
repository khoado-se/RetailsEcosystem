import { useEffect, useRef } from "react";
import Chart from "chart.js/auto";
import "./DashboardPage.css";

export default function DashboardPage() {
  const chartRef = useRef(null);
  const chartInstance = useRef(null);

  useEffect(() => {
    const ctx = chartRef.current;

    chartInstance.current = new Chart(ctx, {
      type: "line",
      data: {
        labels: [
          "Sunday",
          "Monday",
          "Tuesday",
          "Wednesday",
          "Thursday",
          "Friday",
          "Saturday",
        ],
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
        scales: {
          y: {
            beginAtZero: false,
          },
        },
        plugins: {
          legend: {
            display: false,
          },
        },
      },
    });

    return () => {
      chartInstance.current.destroy();
    };
  }, []);

  return (
    <>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <div>
          <h1 className="page-header-title mb-0">Dashboard</h1>
          <p className="page-header-subtitle mb-0">Overview of sales and activity</p>
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
