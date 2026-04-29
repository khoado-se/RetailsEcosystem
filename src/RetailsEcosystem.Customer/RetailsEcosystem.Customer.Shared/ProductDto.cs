namespace RetailsEcosystem.Customer.Shared
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsFeatured { get; set; }
        public int SoldCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public CategoryDto Category { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }
}
