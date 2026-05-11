using RetailsEcosystem.Customer.Shared.DTOs.Order;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IVnpayService
    {
        /// <summary>Builds the signed redirect URL to send the browser to VNPay.</summary>
        string BuildPaymentUrl(OrderDto order, string txnRef, string ipAddress);

        /// <summary>
        /// Validates the HMAC-SHA512 on an inbound parameter dictionary.
        /// Accepts IDictionary (not IQueryCollection) to keep Application free of ASP.NET Core types.
        /// </summary>
        bool ValidateSignature(IDictionary<string, string> parameters);

        /// <summary>Queries VNPay for the current transaction status (optional, for manual verification).</summary>
        Task<VnpayQueryResult> QueryTransactionAsync(string txnRef, string transactionDate, string ipAddress);
    }

    public class VnpayQueryResult
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
