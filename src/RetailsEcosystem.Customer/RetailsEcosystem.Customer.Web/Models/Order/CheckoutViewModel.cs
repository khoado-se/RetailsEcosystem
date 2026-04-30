using System.ComponentModel.DataAnnotations;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;

namespace RetailsEcosystem.Customer.Web.Models.Order
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Shipping address is required.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters.")]
        public string ShippingAddress { get; set; } = string.Empty;

        public CartDto Cart { get; set; } = new();
    }
}
