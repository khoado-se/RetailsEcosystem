namespace RetailsEcosystem.Customer.Shared.DTOs.Order
{
    public class InitiatePaymentResult
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string TxnRef     { get; set; } = string.Empty;
    }
}
