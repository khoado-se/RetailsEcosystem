namespace RetailsEcosystem.Customer.Shared.DTOs.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; }

        public int CategoryId { get; set; }
        public bool IsFeatured { get; set; }
    }
}
