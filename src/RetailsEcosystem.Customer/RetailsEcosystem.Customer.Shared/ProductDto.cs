namespace RetailsEcosystem.Customer.Shared
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public CategoryDto Category { get; set; }
        public ProductImageDto Image { get; set; }
    }
}
