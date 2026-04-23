import { shouldShowLeftDots, shouldShowRightDots } from "../../utils/getPaginationPages.js";
import { getPaginationPages } from "../../utils/getPaginationPages.js";

export default function Pagination({page, setPage, totalPage}) {
    const pages = getPaginationPages(page, totalPage);
    
    return (
        <nav>
        <ul className="pagination justify-content-center">
          {/* PREVIOUS */}
          <li className={`page-item ${page === 1 ? "disabled" : ""}`}>
            <button
              className="page-link"
              onClick={() => setPage(page - 1)}
              disabled={page === 1}
            >
              &laquo;
            </button>
          </li>

          {/* LEFT DOTS */}
          {shouldShowLeftDots(page) && (
            <>
              <li className="page-item">
                <button className="page-link" onClick={() => setPage(1)}>
                  1
                </button>
              </li>

              <li className="page-item disabled">
                <span className="page-link">...</span>
              </li>
            </>
          )}

          {/* PAGE NUMBERS */}
          {pages.map((p) => (
            <li key={p} className={`page-item ${page === p ? "active" : ""}`}>
              <button className="page-link" onClick={() => setPage(p)}>
                {p}
              </button>
            </li>
          ))}

          {/* RIGHT DOTS */}
          {shouldShowRightDots(page, totalPage) && (
            <>
              <li className="page-item disabled">
                <span className="page-link">...</span>
              </li>

              <li className="page-item">
                <button
                  className="page-link"
                  onClick={() => setPage(totalPage)}
                >
                  {totalPage}
                </button>
              </li>
            </>
          )}

          {/* NEXT */}
          <li className={`page-item ${page === totalPage ? "disabled" : ""}`}>
            <button
              className="page-link"
              onClick={() => setPage(page + 1)}
              disabled={page === totalPage}
            >
              &raquo;
            </button>
          </li>
        </ul>
      </nav>
    )
};
