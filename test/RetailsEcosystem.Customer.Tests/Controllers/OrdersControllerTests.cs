using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly Mock<IOrderService> _orderServiceMock = new();
    private readonly OrdersController _sut;

    public OrdersControllerTests()
    {
        _sut = new OrdersController(_orderServiceMock.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "user-123")
                ], "TestAuth"))
            }
        };
    }

    // ── CreateOrder ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrder_Success_ReturnsCreatedAtAction()
    {
        var dto = new CreateOrderDto { ShippingAddress = "123 St" };
        var orderDtoResult = new OrderDto { Id = 10, UserId = "user-123" };
        _orderServiceMock.Setup(s => s.CreateOrderAsync("user-123", dto)).ReturnsAsync(orderDtoResult);

        var result = await _sut.CreateOrder(dto);

        result.Result.Should().BeOfType<CreatedAtActionResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task CreateOrder_ServiceThrowsInvalidOperation_ReturnsBadRequest()
    {
        var dto = new CreateOrderDto();
        _orderServiceMock.Setup(s => s.CreateOrderAsync("user-123", dto))
            .ThrowsAsync(new InvalidOperationException("Cart is empty."));

        var result = await _sut.CreateOrder(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    // ── GetOrder ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrder_NotFound_Returns404()
    {
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(99, "user-123", It.IsAny<string>()))
            .ThrowsAsync(new NotFoundException("Order 99 not found."));

        var result = await _sut.GetOrder(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetOrder_Forbidden_Returns403()
    {
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, "user-123", It.IsAny<string>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _sut.GetOrder(1);

        result.Result.Should().BeOfType<ForbidResult>();
    }

    // ── UpdateStatus ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatus_NotFound_Returns404()
    {
        _orderServiceMock.Setup(s => s.UpdateOrderStatusAsync(99, It.IsAny<UpdateOrderStatusDto>()))
            .ThrowsAsync(new NotFoundException("Not found."));

        var result = await _sut.UpdateStatus(99, new UpdateOrderStatusDto { Status = OrderStatus.Confirmed });

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    // ── UpdateStatus additional ───────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatus_InvalidTransition_ReturnsBadRequest()
    {
        _orderServiceMock.Setup(s => s.UpdateOrderStatusAsync(1, It.IsAny<UpdateOrderStatusDto>()))
            .ThrowsAsync(new InvalidOperationException("Cannot transition from Delivered to Pending."));

        var result = await _sut.UpdateStatus(1, new UpdateOrderStatusDto { Status = OrderStatus.Pending });

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    // ── CancelOrder ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CancelOrder_NotFound_Returns404()
    {
        _orderServiceMock.Setup(s => s.CancelOrderAsync(99, "user-123"))
            .ThrowsAsync(new NotFoundException("Not found."));

        var result = await _sut.CancelOrder(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CancelOrder_Forbidden_Returns403()
    {
        _orderServiceMock.Setup(s => s.CancelOrderAsync(1, "user-123"))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _sut.CancelOrder(1);

        result.Result.Should().BeOfType<ForbidResult>();
    }

    // ── GetOrders ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrders_WhenCalled_ReturnsOkWithPagedResult()
    {
        var pagedResult = new PagedResult<OrderDto>([], new PagedRequest { PageNumber = 1, PageSize = 10 }, 0);
        _orderServiceMock.Setup(s => s.GetOrdersAsync("user-123", It.IsAny<string>(), It.IsAny<PagedRequest>(), null))
            .ReturnsAsync(pagedResult);

        var result = await _sut.GetOrders();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetOrders_WithStatusFilter_PassesStatusToService()
    {
        var pagedResult = new PagedResult<OrderDto>([], new PagedRequest { PageNumber = 1, PageSize = 10 }, 0);
        _orderServiceMock.Setup(s => s.GetOrdersAsync("user-123", It.IsAny<string>(), It.IsAny<PagedRequest>(), OrderStatus.Pending))
            .ReturnsAsync(pagedResult);

        var result = await _sut.GetOrders(status: OrderStatus.Pending);

        result.Result.Should().BeOfType<OkObjectResult>();
        _orderServiceMock.Verify(s => s.GetOrdersAsync("user-123", It.IsAny<string>(), It.IsAny<PagedRequest>(), OrderStatus.Pending), Times.Once);
    }

    // ── GetStats ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetStats_ReturnsOkWithStats()
    {
        var stats = new OrderStatsDto { OrdersToday = 5, PendingOrders = 3 };
        _orderServiceMock.Setup(s => s.GetStatsAsync()).ReturnsAsync(stats);

        var result = await _sut.GetStats();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(stats);
    }

    // ── InitiatePayment ───────────────────────────────────────────────────────

    [Fact]
    public async Task InitiatePayment_Success_ReturnsOkWithPaymentUrl()
    {
        var payResult = new InitiatePaymentResult { PaymentUrl = "https://vnpay/pay?ref=1" };
        _orderServiceMock.Setup(s => s.InitiateVnpayPaymentAsync(1, "user-123", It.IsAny<string>()))
            .ReturnsAsync(payResult);

        var result = await _sut.InitiatePayment(1);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(payResult);
    }

    [Fact]
    public async Task InitiatePayment_OrderNotFound_ReturnsNotFound()
    {
        _orderServiceMock.Setup(s => s.InitiateVnpayPaymentAsync(99, "user-123", It.IsAny<string>()))
            .ThrowsAsync(new NotFoundException("Not found."));

        var result = await _sut.InitiatePayment(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task InitiatePayment_WrongUser_ReturnsForbid()
    {
        _orderServiceMock.Setup(s => s.InitiateVnpayPaymentAsync(2, "user-123", It.IsAny<string>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _sut.InitiatePayment(2);

        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task InitiatePayment_InvalidOperation_ReturnsBadRequest()
    {
        _orderServiceMock.Setup(s => s.InitiateVnpayPaymentAsync(3, "user-123", It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Max attempts reached."));

        var result = await _sut.InitiatePayment(3);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    // ── CancelOrder additional ────────────────────────────────────────────────

    [Fact]
    public async Task CancelOrder_InvalidOperation_ReturnsBadRequest()
    {
        _orderServiceMock.Setup(s => s.CancelOrderAsync(5, "user-123"))
            .ThrowsAsync(new InvalidOperationException("Cannot cancel a delivered order."));

        var result = await _sut.CancelOrder(5);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }
}
