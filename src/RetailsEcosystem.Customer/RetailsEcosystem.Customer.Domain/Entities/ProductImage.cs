using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailsEcosystem.Customer.Domain.Entities
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }

        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
    }
}