using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, int pageNumber, int pageSize);
        Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, OrderStatus? status = null);
        Task<int> GetOrderCountByUserIdAsync(string userId);
        Task<int> GetAllOrderCountAsync(OrderStatus? status = null);
        Task<int> GetOrdersTodayCountAsync();
        Task<decimal> GetRevenueThisMonthAsync();
        Task<int> GetPendingOrderCountAsync();
        Task<IEnumerable<DailyRevenueDto>> GetDailyRevenueAsync(int days);
    }
}
