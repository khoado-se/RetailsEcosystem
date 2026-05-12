using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface IPaymentAttemptRepository
    {
        Task AddAsync(PaymentAttempt attempt);
        Task<PaymentAttempt?> GetByTxnRefAsync(string txnRef);
    }
}
