using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;

        public OrderService(IOrderRepository orderRepo, ICartRepository cartRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
        }

        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            // Validate stock atomically before touching anything
            foreach (var item in cart.Items)
            {
                if (item.Product!.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for '{item.Product.Name}'. Available: {item.Product.StockQuantity}.");
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                Status = OrderStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            foreach (var item in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product!.Name,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                });

                // Decrement stock on the tracked entity (same DbContext scope)
                item.Product.StockQuantity -= item.Quantity;
                item.Product.SoldCount += item.Quantity;
                item.Product.UpdatedDate = DateTime.UtcNow;
            }

            order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            // SaveChanges inside CreateOrderAsync commits order + product stock updates
            await _orderRepo.CreateOrderAsync(order);

            // Clear the cart after the order is persisted
            cart.Items.Clear();
            await _cartRepo.SaveAsync();

            return await GetOrderByIdAsync(order.Id, userId, "Customer");
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(string userId, string role, PagedRequest pageRequest)
        {
            IEnumerable<Order> orders;
            int totalCount;

            if (role == "Admin")
            {
                orders = await _orderRepo.GetAllOrdersAsync(pageRequest.PageNumber, pageRequest.PageSize);
                totalCount = await _orderRepo.GetAllOrderCountAsync();
            }
            else
            {
                orders = await _orderRepo.GetOrdersByUserIdAsync(userId, pageRequest.PageNumber, pageRequest.PageSize);
                totalCount = await _orderRepo.GetOrderCountByUserIdAsync(userId);
            }

            return new PagedResult<OrderDto>(orders.Select(MapToDto), pageRequest, totalCount);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId, string userId, string role)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException($"Order {orderId} not found.");

            if (role != "Admin" && order.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return MapToDto(order);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException($"Order {orderId} not found.");

            order.Status = dto.Status;
            order.UpdatedDate = DateTime.UtcNow;
            await _orderRepo.SaveAsync();
            return MapToDto(order);
        }

        public async Task<OrderDto> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException($"Order {orderId} not found.");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedDate = DateTime.UtcNow;
            await _orderRepo.SaveAsync();
            return MapToDto(order);
        }

        public async Task<OrderStatsDto> GetStatsAsync() => new()
        {
            OrdersToday = await _orderRepo.GetOrdersTodayCountAsync(),
            RevenueThisMonth = await _orderRepo.GetRevenueThisMonthAsync(),
            PendingOrders = await _orderRepo.GetPendingOrderCountAsync()
        };

        private static OrderDto MapToDto(Order order) => new()
        {
            Id = order.Id,
            UserId = order.UserId,
            UserEmail = order.User?.Email ?? string.Empty,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                ProductImageUrl = i.Product?.Images.FirstOrDefault()?.Url
            }).ToList()
        };
    }
}
