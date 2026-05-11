using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Shared.DTOs.Order
{
    public class CreateOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
    }
}
