using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string? VnpayTxnRef { get; set; }
        public string? VnpayTransactionNo { get; set; }
        public int PaymentAttemptCount { get; set; } = 0;
        public DateTime? PaymentExpiresAt { get; set; }
        public DateTime? LastPaymentAttemptAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
