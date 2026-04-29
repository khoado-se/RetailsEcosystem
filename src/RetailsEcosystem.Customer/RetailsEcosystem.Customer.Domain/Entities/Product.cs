using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailsEcosystem.Customer.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 100;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; }

        public Category Category { get; set; } = null!;
        public int CategoryId { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
