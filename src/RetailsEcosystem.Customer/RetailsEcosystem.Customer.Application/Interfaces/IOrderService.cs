using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Order;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<PagedResult<OrderDto>> GetOrdersAsync(string userId, string role, PagedRequest pageRequest);
        Task<OrderDto> GetOrderByIdAsync(int orderId, string userId, string role);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<OrderDto> CancelOrderAsync(int orderId, string userId);
        Task<OrderStatsDto> GetStatsAsync();
    }
}
