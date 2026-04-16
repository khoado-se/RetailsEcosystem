namespace RetailsEcosystem.Customer.Shared
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public ProductDto Product { get; set; }
    }
}
