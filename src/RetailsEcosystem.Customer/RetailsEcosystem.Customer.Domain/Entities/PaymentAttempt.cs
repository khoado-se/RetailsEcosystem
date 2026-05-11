using System.ComponentModel.DataAnnotations;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Domain.Entities
{
    public class PaymentAttempt
    {
        public int    Id        { get; set; }
        public int    OrderId   { get; set; }
        public Order  Order     { get; set; } = null!;

        [MaxLength(100)]
        public string TxnRef    { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? VnpayTxnNo { get; set; }

        [ConcurrencyCheck]
        public PaymentAttemptStatus Status { get; set; } = PaymentAttemptStatus.Initiated;

        [MaxLength(10)]
        public string? ResponseCode { get; set; }

        [MaxLength(50)]
        public string? IpAddress  { get; set; }

        public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt  { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
