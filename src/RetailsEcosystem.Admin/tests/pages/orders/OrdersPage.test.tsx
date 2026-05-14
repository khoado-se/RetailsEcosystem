import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockUseOrders = vi.fn();
const mockGetOrderById = vi.fn();

vi.mock("@features/order/useOrders", () => ({ useOrders: mockUseOrders }));
vi.mock("@features/order/orderApi", () => ({ getOrderById: mockGetOrderById }));
vi.mock("@utils/format", () => ({
  formatCurrency: (v: number) => `${v}₫`,
  formatDate: (d: string) => d ?? "—",
}));
vi.mock("@features/order/OrderDetailModal", () => ({
  default: ({ order, onClose }: { order: unknown; onClose: () => void }) =>
    order ? <div data-testid="order-modal"><button onClick={onClose}>close-modal</button></div> : null,
}));
vi.mock("@components/ui/Pagination", () => ({ default: () => <div data-testid="pagination" /> }));
vi.mock("@components/ui/PageSizeSelector", () => ({ default: () => <div data-testid="page-size" /> }));
vi.mock("react-hot-toast", () => ({ default: { success: vi.fn(), error: vi.fn() } }));

const ORDERS = [
  { id: 1, userId: "u1", userEmail: "a@b.com", totalAmount: 100000, status: 0, paymentMethod: 0, paymentStatus: 0, createdDate: "2026-01-01" },
  { id: 2, userId: "u2", userEmail: "b@c.com", totalAmount: 200000, status: 2, paymentMethod: 1, paymentStatus: 2, createdDate: "2026-01-02" },
];

function mockOrders(overrides = {}) {
  mockUseOrders.mockReturnValue({
    orders: ORDERS, totalPage: 1, loading: false, error: null, fetchOrders: vi.fn(),
    ...overrides,
  });
}

const { default: OrdersPage } = await import("@pages/order/OrdersPage");

describe("OrdersPage — rendering", () => {
  beforeEach(() => { vi.clearAllMocks(); mockOrders(); });

  it("renders Orders heading", () => {
    render(<OrdersPage />);
    expect(screen.getByText("Orders")).toBeInTheDocument();
  });

  it("renders order rows with email", () => {
    render(<OrdersPage />);
    expect(screen.getByText("a@b.com")).toBeInTheDocument();
    expect(screen.getByText("b@c.com")).toBeInTheDocument();
  });

  it("shows error when useOrders returns error", () => {
    mockOrders({ error: "Load failed" });
    render(<OrdersPage />);
    expect(screen.getByText("Load failed")).toBeInTheDocument();
  });

  it("shows loading text while loading", () => {
    mockOrders({ loading: true });
    render(<OrdersPage />);
    expect(screen.getByText("Loading…")).toBeInTheDocument();
  });
});

describe("OrdersPage — interactions", () => {
  beforeEach(() => { vi.clearAllMocks(); mockOrders(); });

  it("opens OrderDetailModal on View button click", async () => {
    mockGetOrderById.mockResolvedValue({ data: ORDERS[0] });
    render(<OrdersPage />);
    // Click the first View button
    const viewButtons = screen.getAllByText("View");
    fireEvent.click(viewButtons[0]);
    await waitFor(() => expect(screen.getByTestId("order-modal")).toBeInTheDocument());
  });

  it("closes OrderDetailModal when onClose called", async () => {
    mockGetOrderById.mockResolvedValue({ data: ORDERS[0] });
    render(<OrdersPage />);
    const viewButtons = screen.getAllByText("View");
    fireEvent.click(viewButtons[0]);
    await waitFor(() => screen.getByTestId("order-modal"));
    fireEvent.click(screen.getByText("close-modal"));
    expect(screen.queryByTestId("order-modal")).not.toBeInTheDocument();
  });

  it("changes status filter when filter button clicked", () => {
    render(<OrdersPage />);
    // Filter buttons are rendered as btn elements; "Pending" appears as both filter btn and table badge
    // Use getAllByRole to find buttons specifically
    const pendingBtns = screen.getAllByRole("button", { name: "Pending" });
    // The first one is the filter button
    fireEvent.click(pendingBtns[0]);
    expect(mockUseOrders).toHaveBeenCalledWith(expect.anything(), 0, expect.anything());
  });

  it("resets to All filter when All clicked", () => {
    render(<OrdersPage />);
    const pendingBtns = screen.getAllByRole("button", { name: "Pending" });
    fireEvent.click(pendingBtns[0]);
    fireEvent.click(screen.getByRole("button", { name: "All" }));
    expect(mockUseOrders).toHaveBeenLastCalledWith(expect.anything(), null, expect.anything());
  });
});
