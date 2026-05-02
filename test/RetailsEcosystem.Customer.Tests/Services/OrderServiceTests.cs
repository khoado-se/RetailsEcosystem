using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;

namespace RetailsEcosystem.Customer.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<ICartRepository> _cartRepoMock = new();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(_orderRepoMock.Object, _cartRepoMock.Object);
    }

    // ── CreateOrderAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrderAsync_NullCart_ThrowsInvalidOperationException()
    {
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync((Domain.Entities.Cart?)null);

        var act = () => _sut.CreateOrderAsync("user1", new CreateOrderDto());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cart is empty.");
    }

    [Fact]
    public async Task CreateOrderAsync_EmptyCartItems_ThrowsInvalidOperationException()
    {
        var cart = new CartBuilder().WithUserId("user1").Build();
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync(cart);

        var act = () => _sut.CreateOrderAsync("user1", new CreateOrderDto());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cart is empty.");
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_ThrowsInvalidOperationException()
    {
        var cart = new CartBuilder()
            .WithUserId("user1")
            .WithItem(productId: 1, quantity: 10, stock: 5)
            .Build();
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync(cart);

        var act = () => _sut.CreateOrderAsync("user1", new CreateOrderDto());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task CreateOrderAsync_ValidCart_CreatesOrderAndDecrementsStock()
    {
        const string userId = "user1";
        var cart = new CartBuilder()
            .WithUserId(userId)
            .WithItem(productId: 1, quantity: 2, unitPrice: 10m, stock: 10)
            .Build();

        // Capture product reference before cart is cleared
        var trackedProduct = cart.Items.First().Product!;

        var createdOrder = new OrderBuilder().WithId(42).WithUserId(userId).WithItem().Build();

        _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cart);
        _orderRepoMock.Setup(r => r.CreateOrderAsync(It.IsAny<Domain.Entities.Order>()))
            .ReturnsAsync(createdOrder);
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(createdOrder);

        var result = await _sut.CreateOrderAsync(userId, new CreateOrderDto { ShippingAddress = "123 St" });

        result.Should().NotBeNull();
        trackedProduct.StockQuantity.Should().Be(8);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidCart_ClearsCartAfterPersist()
    {
        const string userId = "user1";
        var cart = new CartBuilder().WithUserId(userId).WithItem(stock: 100).Build();
        var createdOrder = new OrderBuilder().WithId(1).WithUserId(userId).WithItem().Build();

        _cartRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(cart);
        _orderRepoMock.Setup(r => r.CreateOrderAsync(It.IsAny<Domain.Entities.Order>()))
            .ReturnsAsync(createdOrder);
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(createdOrder);

        await _sut.CreateOrderAsync(userId, new CreateOrderDto());

        cart.Items.Should().BeEmpty();
        _cartRepoMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    // ── GetOrderByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrderByIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(99)).ReturnsAsync((Domain.Entities.Order?)null);

        var act = () => _sut.GetOrderByIdAsync(99, "user1", "Customer");

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task GetOrderByIdAsync_CustomerAccessingOtherUsersOrder_ThrowsUnauthorizedAccessException()
    {
        var order = new OrderBuilder().WithUserId("other-user").Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.GetOrderByIdAsync(1, "user1", "Customer");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetOrderByIdAsync_AdminAccessingAnyOrder_ReturnsDto()
    {
        var order = new OrderBuilder().WithUserId("other-user").Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.GetOrderByIdAsync(1, "admin-id", "Admin");

        result.Should().NotBeNull();
        result.UserId.Should().Be("other-user");
    }

    // ── UpdateOrderStatusAsync ────────────────────────────────────────────────

    [Fact]
    public async Task UpdateOrderStatusAsync_NotFound_ThrowsKeyNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(99)).ReturnsAsync((Domain.Entities.Order?)null);

        var act = () => _sut.UpdateOrderStatusAsync(99, new UpdateOrderStatusDto { Status = OrderStatus.Confirmed });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_InvalidTransition_ThrowsInvalidOperationException()
    {
        var order = new OrderBuilder().WithStatus(OrderStatus.Delivered).Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.UpdateOrderStatusAsync(1, new UpdateOrderStatusDto { Status = OrderStatus.Pending });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot transition*");
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_PendingToConfirmed_Succeeds()
    {
        var order = new OrderBuilder().WithStatus(OrderStatus.Pending).Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.UpdateOrderStatusAsync(1, new UpdateOrderStatusDto { Status = OrderStatus.Confirmed });

        result.Status.Should().Be(OrderStatus.Confirmed);
        _orderRepoMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    // ── CancelOrderAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CancelOrderAsync_WrongUser_ThrowsUnauthorizedAccessException()
    {
        var order = new OrderBuilder().WithUserId("other-user").WithStatus(OrderStatus.Pending).Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.CancelOrderAsync(1, "user1");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task CancelOrderAsync_NonPendingOrder_ThrowsInvalidOperationException()
    {
        var order = new OrderBuilder().WithUserId("user1").WithStatus(OrderStatus.Confirmed).Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var act = () => _sut.CancelOrderAsync(1, "user1");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Only pending orders*");
    }

    [Fact]
    public async Task CancelOrderAsync_ValidPendingOrder_ReturnsCancelledDto()
    {
        var order = new OrderBuilder().WithUserId("user1").WithStatus(OrderStatus.Pending).Build();
        _orderRepoMock.Setup(r => r.GetOrderByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.CancelOrderAsync(1, "user1");

        result.Status.Should().Be(OrderStatus.Cancelled);
    }

    // ── GetOrdersAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrdersAsync_AdminRole_CallsGetAllOrders()
    {
        _orderRepoMock.Setup(r => r.GetAllOrdersAsync(1, 10, null)).ReturnsAsync([]);
        _orderRepoMock.Setup(r => r.GetAllOrderCountAsync(null)).ReturnsAsync(0);

        await _sut.GetOrdersAsync("admin", "Admin", new PagedRequest { PageNumber = 1, PageSize = 10 });

        _orderRepoMock.Verify(r => r.GetAllOrdersAsync(1, 10, null), Times.Once);
        _orderRepoMock.Verify(r => r.GetOrdersByUserIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetOrdersAsync_CustomerRole_CallsGetOrdersByUserId()
    {
        _orderRepoMock.Setup(r => r.GetOrdersByUserIdAsync("user1", 1, 10)).ReturnsAsync([]);
        _orderRepoMock.Setup(r => r.GetOrderCountByUserIdAsync("user1")).ReturnsAsync(0);

        await _sut.GetOrdersAsync("user1", "Customer", new PagedRequest { PageNumber = 1, PageSize = 10 });

        _orderRepoMock.Verify(r => r.GetOrdersByUserIdAsync("user1", 1, 10), Times.Once);
        _orderRepoMock.Verify(r => r.GetAllOrdersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<OrderStatus?>()), Times.Never);
    }

    // ── GetStatsAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetStatsAsync_CallsAllRepoStatMethods()
    {
        _orderRepoMock.Setup(r => r.GetOrdersTodayCountAsync()).ReturnsAsync(5);
        _orderRepoMock.Setup(r => r.GetRevenueThisMonthAsync()).ReturnsAsync(1500m);
        _orderRepoMock.Setup(r => r.GetPendingOrderCountAsync()).ReturnsAsync(3);
        _orderRepoMock.Setup(r => r.GetDailyRevenueAsync(7)).ReturnsAsync([]);

        var result = await _sut.GetStatsAsync();

        result.OrdersToday.Should().Be(5);
        result.RevenueThisMonth.Should().Be(1500m);
        result.PendingOrders.Should().Be(3);
        _orderRepoMock.Verify(r => r.GetOrdersTodayCountAsync(), Times.Once);
        _orderRepoMock.Verify(r => r.GetRevenueThisMonthAsync(), Times.Once);
        _orderRepoMock.Verify(r => r.GetPendingOrderCountAsync(), Times.Once);
        _orderRepoMock.Verify(r => r.GetDailyRevenueAsync(7), Times.Once);
    }
}
