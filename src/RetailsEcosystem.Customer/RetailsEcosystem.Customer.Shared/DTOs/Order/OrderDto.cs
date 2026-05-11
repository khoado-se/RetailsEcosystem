using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Shared.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string StatusLabel => Status.ToString();
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? VnpayTxnRef { get; set; }
        public string? VnpayTransactionNo { get; set; }
        public DateTime? PaymentExpiresAt { get; set; }
        public int PaymentAttemptCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
