namespace RetailsEcosystem.Customer.Shared.DTOs
{
    public class PagedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? Search { get; set; }
    }
}
