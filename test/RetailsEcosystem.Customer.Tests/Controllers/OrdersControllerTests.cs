using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.Application.Interfaces;
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
        var orderDto = new OrderBuilder().WithId(10).Build();
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
            .ThrowsAsync(new KeyNotFoundException("Order 99 not found."));

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
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _sut.UpdateStatus(99, new UpdateOrderStatusDto { Status = OrderStatus.Confirmed });

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    // ── CancelOrder ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CancelOrder_NotFound_Returns404()
    {
        _orderServiceMock.Setup(s => s.CancelOrderAsync(99, "user-123"))
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _sut.CancelOrder(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }
}
