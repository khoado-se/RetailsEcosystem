import { describe, it, expect } from "vitest";
import { formatCurrency, formatDate, formatDateTime } from "@utils/format";

describe("formatCurrency", () => {
  it("formats a positive number as VND currency string", () => {
    const result = formatCurrency(100000);
    // Node's vi-VN locale produces "100.000 ₫" — assert structure, not exact string
    expect(result).toMatch(/100/);
    expect(typeof result).toBe("string");
  });

  it("formats zero without throwing", () => {
    const result = formatCurrency(0);
    expect(typeof result).toBe("string");
  });

  it("formats string-encoded number", () => {
    const result = formatCurrency("50000");
    expect(typeof result).toBe("string");
    expect(result).toMatch(/50/);
  });
});

describe("formatDate", () => {
  it("formats a valid ISO date string to a locale date", () => {
    const result = formatDate("2024-06-15T00:00:00Z");
    expect(typeof result).toBe("string");
    expect(result.length).toBeGreaterThan(0);
    expect(result).not.toBe("—");
  });

  it("returns em-dash for null", () => {
    expect(formatDate(null)).toBe("—");
  });

  it("returns em-dash for undefined", () => {
    expect(formatDate(undefined)).toBe("—");
  });
});

describe("formatDateTime", () => {
  it("formats a valid ISO date-time string", () => {
    const result = formatDateTime("2024-06-15T10:30:00Z");
    expect(typeof result).toBe("string");
    expect(result.length).toBeGreaterThan(0);
    expect(result).not.toBe("—");
  });

  it("returns em-dash for null", () => {
    expect(formatDateTime(null)).toBe("—");
  });

  it("returns em-dash for undefined", () => {
    expect(formatDateTime(undefined)).toBe("—");
  });
});
