using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories
{
    public class PaymentAttemptRepository : IPaymentAttemptRepository
    {
        private readonly AppDbContext _context;

        public PaymentAttemptRepository(AppDbContext context) => _context = context;

        public Task AddAsync(PaymentAttempt attempt)
        {
            _context.PaymentAttempts.Add(attempt);
            return Task.CompletedTask;
        }

        public Task<PaymentAttempt?> GetByTxnRefAsync(string txnRef) =>
            _context.PaymentAttempts.FirstOrDefaultAsync(a => a.TxnRef == txnRef);
    }
}
