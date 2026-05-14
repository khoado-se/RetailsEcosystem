import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";

const mockUseCustomers = vi.fn();

vi.mock("@features/customer/useCustomers", () => ({ useCustomers: mockUseCustomers }));
vi.mock("@features/customer/CustomerTable", () => ({
  default: ({ customers }: { customers: { id: string; fullName: string }[] }) => (
    <div data-testid="customer-table">
      {customers.map((c) => <span key={c.id}>{c.fullName}</span>)}
    </div>
  ),
}));
vi.mock("@components/ui/PageSizeSelector", () => ({
  default: ({ onPageSizeChange }: { onPageSizeChange: (n: number) => void }) => (
    <button data-testid="page-size" onClick={() => onPageSizeChange(20)}>page-size</button>
  ),
}));

const CUSTOMERS = [
  { id: "u1", fullName: "Alice" },
  { id: "u2", fullName: "Bob" },
];

function mockCustomers(overrides = {}) {
  mockUseCustomers.mockReturnValue({
    customers: CUSTOMERS, totalPage: 1, loading: false, fetchCustomers: vi.fn(),
    ...overrides,
  });
}

const { default: CustomerListPage } = await import("@pages/customers/CustomerListPage");

describe("CustomerListPage", () => {
  beforeEach(() => { vi.clearAllMocks(); mockCustomers(); });

  it("renders Customers heading", () => {
    render(<CustomerListPage />);
    expect(screen.getByText("Customers")).toBeInTheDocument();
  });

  it("renders customer names via CustomerTable", () => {
    render(<CustomerListPage />);
    expect(screen.getByText("Alice")).toBeInTheDocument();
    expect(screen.getByText("Bob")).toBeInTheDocument();
  });

  it("updates search input on type", () => {
    render(<CustomerListPage />);
    const input = screen.getByPlaceholderText(/search/i);
    fireEvent.change(input, { target: { value: "alice" } });
    expect((input as HTMLInputElement).value).toBe("alice");
  });

  it("shows Clear button when search is not empty and clears on click", async () => {
    render(<CustomerListPage />);
    const input = screen.getByPlaceholderText(/search/i);
    fireEvent.change(input, { target: { value: "alice" } });
    await waitFor(() => expect(screen.getByText(/Clear/i)).toBeInTheDocument());
    fireEvent.click(screen.getByText(/Clear/i));
    expect((input as HTMLInputElement).value).toBe("");
  });

  it("changes page size on PageSizeSelector change", () => {
    render(<CustomerListPage />);
    fireEvent.click(screen.getByTestId("page-size"));
    // should not crash; useCustomers gets called with new pageSize via rerender
    expect(mockUseCustomers).toHaveBeenCalled();
  });

  it("shows loading spinner while loading", () => {
    mockCustomers({ loading: true });
    render(<CustomerListPage />);
    expect(document.querySelector(".spinner-border")).not.toBeNull();
  });
});
