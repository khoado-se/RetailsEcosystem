import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockUpdateStatus = vi.fn();
const mockToast = { success: vi.fn(), error: vi.fn() };

vi.mock("@features/customer/customerApi", () => ({ updateCustomerStatus: mockUpdateStatus }));
vi.mock("react-hot-toast", () => ({ default: mockToast }));
vi.mock("@components/ui/Pagination", () => ({ default: () => <div data-testid="pagination" /> }));

const { default: CustomerTable } = await import("@features/customer/CustomerTable");

const CUSTOMERS = [
  { id: "u1", fullName: "Alice", email: "alice@test.com", phoneNumber: "0900", roles: ["Customer"], isActive: true },
  { id: "u2", fullName: "Bob", email: "bob@test.com", phoneNumber: null, roles: [], isActive: false },
];

function renderTable(overrides = {}) {
  const props = {
    customers: CUSTOMERS,
    pageNumber: 1,
    setPageNumber: vi.fn(),
    totalPage: 1,
    onStatusChange: vi.fn(),
    ...overrides,
  };
  return { ...render(<CustomerTable {...props} />), props };
}

describe("CustomerTable — rendering", () => {
  it("shows empty state when no customers", () => {
    renderTable({ customers: [] });
    expect(screen.getByText("No customers found.")).toBeInTheDocument();
  });

  it("renders customer names and emails", () => {
    renderTable();
    expect(screen.getByText("Alice")).toBeInTheDocument();
    expect(screen.getByText("bob@test.com")).toBeInTheDocument();
  });

  it("shows — for missing phoneNumber", () => {
    renderTable();
    expect(screen.getByText("—")).toBeInTheDocument();
  });

  it("shows Active badge for active customer", () => {
    renderTable();
    expect(screen.getByText("Active")).toBeInTheDocument();
  });

  it("shows Inactive badge for inactive customer", () => {
    renderTable();
    expect(screen.getByText("Inactive")).toBeInTheDocument();
  });

  it("shows Deactivate button for active customer", () => {
    renderTable();
    expect(screen.getByText("Deactivate")).toBeInTheDocument();
  });

  it("shows Activate button for inactive customer", () => {
    renderTable();
    expect(screen.getByText("Activate")).toBeInTheDocument();
  });
});

describe("CustomerTable — toggle status", () => {
  beforeEach(() => vi.clearAllMocks());

  it("calls updateCustomerStatus and onStatusChange on success", async () => {
    mockUpdateStatus.mockResolvedValue({});
    const { props } = renderTable();
    fireEvent.click(screen.getByText("Deactivate"));
    await waitFor(() => expect(mockUpdateStatus).toHaveBeenCalledWith("u1", false));
    expect(props.onStatusChange).toHaveBeenCalled();
    expect(mockToast.success).toHaveBeenCalled();
  });

  it("shows error toast when update fails", async () => {
    mockUpdateStatus.mockRejectedValue(new Error("fail"));
    renderTable();
    fireEvent.click(screen.getByText("Deactivate"));
    await waitFor(() => expect(mockToast.error).toHaveBeenCalled());
  });

  it("shows ... while updating", async () => {
    let resolve: (v: unknown) => void;
    mockUpdateStatus.mockReturnValue(new Promise((r) => { resolve = r; }));
    renderTable();
    fireEvent.click(screen.getByText("Deactivate"));
    await waitFor(() => expect(screen.getByText("...")).toBeInTheDocument());
    resolve!(undefined);
  });
});
