namespace RetailsEcosystem.Customer.Shared.Settings
{
    public class VnpaySettings
    {
        public string TmnCode { get; set; } = null!;
        public string HashSecret { get; set; } = null!;
        public string PaymentUrl { get; set; } = null!;
        public string QueryDrUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
    }
}
