namespace RetailsEcosystem.Customer.Web.Models.Vnpay
{
    public class VnpayReturnViewModel
    {
        public bool   Success      { get; set; }
        public bool   Cancelled    { get; set; }
        public bool   Retryable    { get; set; }
        public int    OrderId      { get; set; }
        public int    AttemptsLeft { get; set; }
        public long   Amount       { get; set; }
        public string Message      { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
    }
}
