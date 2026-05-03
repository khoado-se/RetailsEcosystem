using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class CartsControllerTests
{
    private readonly Mock<ICartService> _cartServiceMock = new();
    private readonly CartsController _sut;

    public CartsControllerTests()
    {
        _sut = new CartsController(_cartServiceMock.Object);
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

    // ── GetCart ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetCart_WhenCalled_ReturnsOkWithCartDto()
    {
        var cartDto = new CartDto { Id = 1, Items = [] };
        _cartServiceMock.Setup(s => s.GetCartAsync("user-123")).ReturnsAsync(cartDto);

        var result = await _sut.GetCart();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(cartDto);
    }

    // ── AddItem ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddItem_ValidRequest_ReturnsOkWithCartDto()
    {
        var cartDto = new CartDto { Id = 1, Items = [] };
        _cartServiceMock.Setup(s => s.AddItemAsync("user-123", It.IsAny<AddCartItemDto>()))
            .ReturnsAsync(cartDto);

        var result = await _sut.AddItem(new AddCartItemDto { ProductId = 1, Quantity = 2 });

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task AddItem_ProductNotFound_Returns404()
    {
        _cartServiceMock.Setup(s => s.AddItemAsync("user-123", It.IsAny<AddCartItemDto>()))
            .ThrowsAsync(new KeyNotFoundException("Product 99 not found."));

        var result = await _sut.AddItem(new AddCartItemDto { ProductId = 99, Quantity = 1 });

        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task AddItem_InsufficientStock_Returns400()
    {
        _cartServiceMock.Setup(s => s.AddItemAsync("user-123", It.IsAny<AddCartItemDto>()))
            .ThrowsAsync(new InvalidOperationException("Insufficient stock."));

        var result = await _sut.AddItem(new AddCartItemDto { ProductId = 1, Quantity = 50 });

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    // ── UpdateItem ────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateItem_WrongUser_Returns403()
    {
        _cartServiceMock.Setup(s => s.UpdateItemAsync("user-123", 5, It.IsAny<UpdateCartItemDto>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _sut.UpdateItem(5, new UpdateCartItemDto { Quantity = 2 });

        result.Result.Should().BeOfType<ForbidResult>();
    }

    // ── RemoveItem ────────────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveItem_NotFound_Returns404()
    {
        _cartServiceMock.Setup(s => s.RemoveItemAsync("user-123", 5))
            .ThrowsAsync(new KeyNotFoundException("Cart item 5 not found."));

        var result = await _sut.RemoveItem(5);

        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
    }

    // ── ClearCart ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task ClearCart_WhenCalled_ReturnsNoContent()
    {
        _cartServiceMock.Setup(s => s.ClearCartAsync("user-123")).Returns(Task.CompletedTask);

        var result = await _sut.ClearCart();

        result.Should().BeOfType<NoContentResult>();
    }
}
