using RetailsEcosystem.Customer.Shared.DTOs.Order;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface IVnpayWebService
    {
        string BuildPaymentUrl(OrderDto order, string txnRef, string ipAddress);
        bool ValidateSignature(IDictionary<string, string> parameters);
    }
}
