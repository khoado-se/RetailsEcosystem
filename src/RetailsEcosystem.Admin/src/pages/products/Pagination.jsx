import { shouldShowLeftDots, shouldShowRightDots } from "../../utils/getPaginationPages.js";
import { getPaginationPages } from "../../utils/getPaginationPages.js";

export default function Pagination({pageNumber, setPageNumber, totalPage}) {
    const pages = getPaginationPages(pageNumber, totalPage);
    
    return (
        <nav>
        <ul className="pagination justify-content-center">
          {/* PREVIOUS */}
          <li className={`page-item ${pageNumber === 1 ? "disabled" : ""}`}>
            <button
              className="page-link"
              onClick={() => setPageNumber(pageNumber - 1)}
              disabled={pageNumber === 1}
            >
              &laquo;
            </button>
          </li>

          {/* LEFT DOTS */}
          {shouldShowLeftDots(pageNumber) && (
            <>
              <li className="page-item">
                <button className="page-link" onClick={() => setPageNumber(1)}>
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
            <li key={p} className={`page-item ${pageNumber === p ? "active" : ""}`}>
              <button className="page-link" onClick={() => setPageNumber(p)}>
                {p}
              </button>
            </li>
          ))}

          {/* RIGHT DOTS */}
          {shouldShowRightDots(pageNumber, totalPage) && (
            <>
              <li className="page-item disabled">
                <span className="page-link">...</span>
              </li>

              <li className="page-item">
                <button
                  className="page-link"
                  onClick={() => setPageNumber(totalPage)}
                >
                  {totalPage}
                </button>
              </li>
            </>
          )}

          {/* NEXT */}
          <li className={`page-item ${pageNumber === totalPage ? "disabled" : ""}`}>
            <button
              className="page-link"
              onClick={() => setPageNumber(pageNumber + 1)}
              disabled={pageNumber === totalPage}
            >
              &raquo;
            </button>
          </li>
        </ul>
      </nav>
    )
};
