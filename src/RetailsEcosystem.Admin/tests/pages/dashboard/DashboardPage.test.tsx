import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";

const mockGetOrderStats = vi.fn();
vi.mock("@features/order/orderApi", () => ({ getOrderStats: mockGetOrderStats }));
vi.mock("@utils/format", () => ({
  formatCurrency: (v: number) => `${v}₫`,
}));
vi.mock("chart.js/auto", () => ({
  default: class {
    static defaults = { font: {} };
    constructor() {}
    destroy() {}
    update() {}
  },
}));

const { default: DashboardPage } = await import("@pages/dashboard/DashboardPage");

const STATS = {
  totalOrders: 100,
  pendingOrders: 20,
  totalRevenue: 5000000,
  totalProducts: 50,
  totalCustomers: 30,
  ordersToday: 5,
  revenueThisMonth: 1000000,
  dailyRevenue: [
    { date: "2026-01-01", revenue: 100000 },
    { date: "2026-01-02", revenue: 200000 },
  ],
};

describe("DashboardPage", () => {
  beforeEach(() => vi.clearAllMocks());

  it("shows loading state initially", () => {
    mockGetOrderStats.mockReturnValue(new Promise(() => {}));
    render(<DashboardPage />);
    expect(document.querySelector(".spinner-border")).not.toBeNull();
  });

  it("renders stats cards after data loads", async () => {
    mockGetOrderStats.mockResolvedValue({ data: STATS });
    render(<DashboardPage />);
    // Dashboard shows ordersToday (5) and pendingOrders (20) KPI cards
    await waitFor(() => expect(screen.getByText("5")).toBeInTheDocument());
    expect(screen.getByText("20")).toBeInTheDocument();
  });

  it("shows error message when stats fetch fails", async () => {
    mockGetOrderStats.mockRejectedValue(new Error("Network error"));
    render(<DashboardPage />);
    await waitFor(() => expect(screen.getByText("Network error")).toBeInTheDocument());
  });

  it("renders page title", async () => {
    mockGetOrderStats.mockResolvedValue({ data: STATS });
    render(<DashboardPage />);
    await waitFor(() => expect(screen.getByText(/Dashboard/i)).toBeInTheDocument());
  });
});
