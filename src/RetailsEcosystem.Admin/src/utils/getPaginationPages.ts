export function getPaginationPages(
  currentPage,
  totalPages,
  maxVisiblePages = 5
) {
  if (totalPages <= 0) return [];

  const pages = [];

  const halfLeft = Math.floor(maxVisiblePages / 2);
  const halfRight = Math.floor(maxVisiblePages / 2);
  const nearStartLimit = Math.ceil(maxVisiblePages / 2);
  const nearEndLimit = totalPages - halfRight;

  let startPage = currentPage - halfLeft;
  let endPage = currentPage + halfRight;

  // Near the beginning: show first pages
  if (currentPage <= nearStartLimit) {
    startPage = 1;
    endPage = Math.min(totalPages, maxVisiblePages);
  }

  // Near the end: show last pages
  else if (currentPage > nearEndLimit) {
    startPage = Math.max(1, totalPages - maxVisiblePages + 1);
    endPage = totalPages;
  }

  // Middle range
  else {
    startPage = currentPage - halfLeft;
    endPage = currentPage + halfRight;
  }

  for (let page = startPage; page <= endPage; page++) {
    pages.push(page);
  }

  return pages;
}

export function shouldShowLeftDots(
  currentPage,
  maxVisiblePages = 5
) {
  const nearStartLimit = Math.ceil(maxVisiblePages / 2);
  return currentPage > nearStartLimit;
}

export function shouldShowRightDots(
  currentPage,
  totalPages,
  maxVisiblePages = 5
) {
  const halfRight = Math.floor(maxVisiblePages / 2);
  const nearEndLimit = totalPages - halfRight;

  return currentPage < nearEndLimit;
}