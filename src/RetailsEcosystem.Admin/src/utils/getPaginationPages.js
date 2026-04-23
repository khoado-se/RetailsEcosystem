export function getPaginationPages(page, totalPage, maxVisible = 5) {
  if (totalPage <= 0) return [];

  const pages = [];

  let start = Math.max(1, page - Math.floor(maxVisible / 2));
  let end = Math.min(totalPage, page + Math.floor(maxVisible / 2));

  // fix first
  if (page <= Math.ceil(maxVisible / 2)) {
    start = 1;
    end = Math.min(totalPage, maxVisible);
  }

  // fix last
  if (page > totalPage - Math.floor(maxVisible / 2)) {
    start = Math.max(1, totalPage - maxVisible + 1);
    end = totalPage;
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
}

export function shouldShowLeftDots(page, maxVisible = 5) {
  return page > Math.ceil(maxVisible / 2);
}

export function shouldShowRightDots(page, totalPage, maxVisible = 5) {
  return page < totalPage - Math.floor(maxVisible / 2);
}