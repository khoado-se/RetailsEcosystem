namespace RetailsEcosystem.Customer.Shared
{
    public class PageResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPage { get; set; }
    }
}
