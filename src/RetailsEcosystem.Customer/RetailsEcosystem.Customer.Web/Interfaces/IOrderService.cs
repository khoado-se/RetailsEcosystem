using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs.Order;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string accessToken, CreateOrderDto dto);
        Task<PagedResult<OrderDto>> GetOrdersAsync(string accessToken, int pageNumber, int pageSize);
        Task<OrderDto> GetOrderByIdAsync(string accessToken, int orderId);
        Task<OrderDto> CancelOrderAsync(string accessToken, int orderId);
    }
}
