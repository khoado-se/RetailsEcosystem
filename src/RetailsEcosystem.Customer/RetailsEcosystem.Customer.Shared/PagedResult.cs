using RetailsEcosystem.Customer.Shared.DTOs;

namespace RetailsEcosystem.Customer.Shared
{
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }

        public PagedResult() { }
        public PagedResult(IEnumerable<T> Items, PagedRequest pagedRequest, int totalItems)
        {
            this.Items = Items;
            PageNumber = pagedRequest.PageNumber;
            PageSize = pagedRequest.PageSize;
            TotalPage = (int)Math.Ceiling((double)totalItems / pagedRequest.PageSize);
            TotalCount = totalItems;
        }
    }
}
