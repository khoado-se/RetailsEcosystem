using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<PagedResult<OrderDto>> GetOrdersAsync(string userId, string role, PagedRequest pageRequest, OrderStatus? status = null);
        Task<OrderDto> GetOrderByIdAsync(int orderId, string userId, string role);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<OrderDto> CancelOrderAsync(int orderId, string userId);
        Task<OrderStatsDto> GetStatsAsync();

        /// <summary>Builds payment URL, creates a PaymentAttempt, sets order to AwaitingPayment.</summary>
        Task<InitiatePaymentResult> InitiateVnpayPaymentAsync(int orderId, string userId, string ipAddress);

        /// <summary>Called by IPN after VNPay confirms payment. Idempotent.</summary>
        Task ConfirmVnpayPaymentAsync(int orderId, string vnpayTransactionNo, string txnRef);

        /// <summary>Called by IPN when VNPay reports failure or cancellation.</summary>
        Task RecordPaymentOutcomeAsync(int orderId, string txnRef, string responseCode, PaymentAttemptStatus attemptStatus);

        /// <summary>Looks up a PaymentAttempt by TxnRef for IPN idempotency check.</summary>
        Task<PaymentAttempt?> GetPaymentAttemptByTxnRefAsync(string txnRef);
    }
}
