import "@testing-library/jest-dom";

// IntersectionObserver is not implemented in jsdom
(globalThis as unknown as Record<string, unknown>).IntersectionObserver = class {
  observe() {}
  unobserve() {}
  disconnect() {}
};

// ResizeObserver is not implemented in jsdom
(globalThis as unknown as Record<string, unknown>).ResizeObserver = class {
  observe() {}
  unobserve() {}
  disconnect() {}
};
