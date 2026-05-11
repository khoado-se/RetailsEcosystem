using System.ComponentModel.DataAnnotations;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Web.Models.Order
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Shipping address is required.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters.")]
        public string ShippingAddress { get; set; } = string.Empty;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

        public CartDto Cart { get; set; } = new();
    }
}
