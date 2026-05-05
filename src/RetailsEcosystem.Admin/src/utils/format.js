const LOCALE = "vi-VN";
const CURRENCY = "VND";

export const formatCurrency = (value) =>
  Number(value).toLocaleString(LOCALE, {
    style: "currency",
    currency: CURRENCY,
    maximumFractionDigits: 0,
  });

export const formatDate = (date) =>
  date ? new Date(date).toLocaleDateString(LOCALE, { dateStyle: "medium" }) : "—";

export const formatDateTime = (date) =>
  date ? new Date(date).toLocaleString(LOCALE, { dateStyle: "medium", timeStyle: "short" }) : "—";
