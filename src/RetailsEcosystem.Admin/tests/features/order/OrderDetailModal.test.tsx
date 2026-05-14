import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockModalInstance = { show: vi.fn(), hide: vi.fn() };
const mockUpdateOrderStatus = vi.fn();

vi.mock("bootstrap", () => ({ Modal: { getOrCreateInstance: vi.fn(() => mockModalInstance) } }));
vi.mock("@features/order/orderApi", () => ({ updateOrderStatus: mockUpdateOrderStatus }));
vi.mock("@utils/format", () => ({
  formatCurrency: (v: number) => `${v}₫`,
  formatDateTime: (d: string) => d ?? "—",
}));

const { default: OrderDetailModal } = await import("@features/order/OrderDetailModal");

const ORDER_PENDING = {
  id: 1, userId: "u1", userEmail: "u@test.com", shippingAddress: "123 St",
  totalAmount: 500000, createdDate: "2026-01-01", status: 0,
  paymentMethod: 0, paymentStatus: 0,
  items: [{ id: 1, productName: "Phone", quantity: 1, unitPrice: 500000, lineTotal: 500000 }],
};

const ORDER_CONFIRMED = { ...ORDER_PENDING, id: 2, status: 1 };
const ORDER_DELIVERED = { ...ORDER_PENDING, id: 3, status: 3 };

function renderModal(order = ORDER_PENDING, onStatusUpdated = vi.fn(), onClose = vi.fn()) {
  return { ...render(<OrderDetailModal order={order} onStatusUpdated={onStatusUpdated} onClose={onClose} />), onStatusUpdated, onClose };
}

describe("OrderDetailModal — show/hide", () => {
  beforeEach(() => vi.clearAllMocks());

  it("calls show() when order is provided", () => {
    renderModal(ORDER_PENDING);
    expect(mockModalInstance.show).toHaveBeenCalled();
  });

  it("calls hide() when order is null", () => {
    renderModal(null as unknown as typeof ORDER_PENDING);
    expect(mockModalInstance.hide).toHaveBeenCalled();
  });
});

describe("OrderDetailModal — content rendering", () => {
  beforeEach(() => vi.clearAllMocks());

  it("renders order id, customer email, and item names", () => {
    renderModal(ORDER_PENDING);
    expect(screen.getByText("Order #1")).toBeInTheDocument();
    expect(screen.getByText("u@test.com")).toBeInTheDocument();
    expect(screen.getByText("Phone")).toBeInTheDocument();
  });

  it("shows Payment Method COD for paymentMethod=0", () => {
    renderModal(ORDER_PENDING);
    expect(screen.getByText("COD")).toBeInTheDocument();
  });

  it("shows terminal message for Delivered order (no status change buttons)", () => {
    renderModal(ORDER_DELIVERED);
    expect(screen.getByText("No further status changes allowed.")).toBeInTheDocument();
  });

  it("shows Confirm Order button for Pending status", () => {
    renderModal(ORDER_PENDING);
    expect(screen.getByText(/Confirm Order/)).toBeInTheDocument();
  });

  it("shows Cancel Order button for Pending and Confirmed orders", () => {
    renderModal(ORDER_PENDING);
    expect(screen.getByText("Cancel Order")).toBeInTheDocument();
  });

  it("shows VNPay badge for paymentMethod=1", () => {
    renderModal({ ...ORDER_PENDING, paymentMethod: 1 });
    expect(screen.getByText("VNPay")).toBeInTheDocument();
  });
});

describe("OrderDetailModal — VNPay unpaid filter", () => {
  beforeEach(() => vi.clearAllMocks());

  it("hides Confirm Order button for VNPay+unpaid Pending order", () => {
    renderModal({ ...ORDER_PENDING, paymentMethod: 1, paymentStatus: 0 });
    expect(screen.queryByText(/Confirm Order/)).not.toBeInTheDocument();
  });

  it("shows Confirm Order button for VNPay+paid Pending order", () => {
    renderModal({ ...ORDER_PENDING, paymentMethod: 1, paymentStatus: 2 });
    expect(screen.getByText(/Confirm Order/)).toBeInTheDocument();
  });
});

describe("OrderDetailModal — status change", () => {
  beforeEach(() => vi.clearAllMocks());

  it("opens confirm modal when next-status button clicked", () => {
    renderModal(ORDER_PENDING);
    fireEvent.click(screen.getByText(/Confirm Order/));
    expect(mockModalInstance.show).toHaveBeenCalledTimes(2); // detail + confirm
  });

  it("calls updateOrderStatus and onStatusUpdated on confirm", async () => {
    mockUpdateOrderStatus.mockResolvedValue({});
    const { onStatusUpdated } = renderModal(ORDER_PENDING);
    fireEvent.click(screen.getByText(/Confirm Order/));
    await waitFor(() => expect(screen.getByText("Confirm")).toBeInTheDocument());
    fireEvent.click(screen.getByText("Confirm"));
    await waitFor(() => expect(mockUpdateOrderStatus).toHaveBeenCalledWith(1, 1));
    expect(onStatusUpdated).toHaveBeenCalled();
  });

  it("shows error when updateOrderStatus fails", async () => {
    mockUpdateOrderStatus.mockRejectedValue({ response: { data: { detail: "Transition invalid" } } });
    renderModal(ORDER_PENDING);
    fireEvent.click(screen.getByText(/Confirm Order/));
    await waitFor(() => screen.getByText("Confirm"));
    fireEvent.click(screen.getByText("Confirm"));
    await waitFor(() => expect(screen.getByText("Transition invalid")).toBeInTheDocument());
  });

  it("shows Confirm with Danger button for Cancel action", () => {
    renderModal(ORDER_PENDING);
    fireEvent.click(screen.getByText("Cancel Order"));
    expect(mockModalInstance.show).toHaveBeenCalledTimes(2);
  });
});
