using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;

namespace RetailsEcosystem.Customer.Tests.Services;

public class CartServiceTests
{
    private readonly Mock<ICartRepository> _cartRepoMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CartService _sut;

    public CartServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _sut = new CartService(_cartRepoMock.Object, _productRepoMock.Object, _unitOfWorkMock.Object);
    }

    // ── AddItemAsync ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task AddItemAsync_ZeroOrNegativeQuantity_ThrowsArgumentException(int qty)
    {
        var act = () => _sut.AddItemAsync("user1", new AddCartItemDto { ProductId = 1, Quantity = qty });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Quantity*");
    }

    [Fact]
    public async Task AddItemAsync_ProductNotFound_ThrowsNotFoundException()
    {
        _productRepoMock.Setup(r => r.GetProductByIdAsync(99)).ReturnsAsync((Product?)null);

        var act = () => _sut.AddItemAsync("user1", new AddCartItemDto { ProductId = 99, Quantity = 1 });

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task AddItemAsync_InsufficientStock_ThrowsInvalidOperationException()
    {
        var product = new ProductBuilder().WithId(1).WithStock(3).Build();
        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        var act = () => _sut.AddItemAsync("user1", new AddCartItemDto { ProductId = 1, Quantity = 5 });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public async Task AddItemAsync_NewItem_AddsToCart()
    {
        var product = new ProductBuilder().WithId(1).WithStock(10).WithPrice(20m).Build();
        var cart = new CartBuilder().WithUserId("user1").Build();

        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);
        _cartRepoMock.Setup(r => r.GetOrCreateAsync("user1")).ReturnsAsync(cart);

        var result = await _sut.AddItemAsync("user1", new AddCartItemDto { ProductId = 1, Quantity = 3 });

        result.Items.Should().HaveCount(1);
        result.Items.First().Quantity.Should().Be(3);
    }

    [Fact]
    public async Task AddItemAsync_ExistingItem_AccumulatesQuantity()
    {
        var product = new ProductBuilder().WithId(1).WithStock(20).WithPrice(10m).Build();
        var cart = new CartBuilder().WithUserId("user1").WithItem(productId: 1, quantity: 3, stock: 20).Build();

        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);
        _cartRepoMock.Setup(r => r.GetOrCreateAsync("user1")).ReturnsAsync(cart);

        var result = await _sut.AddItemAsync("user1", new AddCartItemDto { ProductId = 1, Quantity = 4 });

        result.Items.First().Quantity.Should().Be(7);
    }

    [Fact]
    public async Task AddItemAsync_ExistingItem_AccumulatedQtyExceedsStock_Throws()
    {
        var product = new ProductBuilder().WithId(1).WithStock(5).Build();
        var cart = new CartBuilder().WithUserId("user1").WithItem(productId: 1, quantity: 3, stock: 5).Build();

        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);
        _cartRepoMock.Setup(r => r.GetOrCreateAsync("user1")).ReturnsAsync(cart);

        var act = () => _sut.AddItemAsync("user1", new AddCartItemDto { ProductId = 1, Quantity = 4 });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }

    // ── UpdateItemAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateItemAsync_ZeroQty_ThrowsArgumentException()
    {
        var act = () => _sut.UpdateItemAsync("user1", 1, new UpdateCartItemDto { Quantity = 0 });

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateItemAsync_ItemNotBelongingToUser_ThrowsUnauthorizedAccessException()
    {
        var cart = new CartBuilder().WithUserId("other-user").WithItem().Build();
        var item = cart.Items.First();
        _cartRepoMock.Setup(r => r.GetItemAsync(item.Id)).ReturnsAsync(item);

        var act = () => _sut.UpdateItemAsync("user1", item.Id, new UpdateCartItemDto { Quantity = 2 });

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task UpdateItemAsync_InsufficientStock_ThrowsInvalidOperationException()
    {
        var cart = new CartBuilder().WithUserId("user1").WithItem(productId: 1, quantity: 2, stock: 3).Build();
        var item = cart.Items.First();
        var product = new ProductBuilder().WithId(1).WithStock(3).Build();

        _cartRepoMock.Setup(r => r.GetItemAsync(item.Id)).ReturnsAsync(item);
        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        var act = () => _sut.UpdateItemAsync("user1", item.Id, new UpdateCartItemDto { Quantity = 5 });

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // ── RemoveItemAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveItemAsync_ItemNotBelongingToUser_ThrowsUnauthorizedAccessException()
    {
        var cart = new CartBuilder().WithUserId("other-user").WithItem().Build();
        var item = cart.Items.First();
        _cartRepoMock.Setup(r => r.GetItemAsync(item.Id)).ReturnsAsync(item);

        var act = () => _sut.RemoveItemAsync("user1", item.Id);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    // ── ClearCartAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ClearCartAsync_NullCart_DoesNotThrow()
    {
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync((Cart?)null);

        var act = () => _sut.ClearCartAsync("user1");

        await act.Should().NotThrowAsync();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── GetCartAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetCartAsync_ExistingUser_ReturnsCartWithItems()
    {
        var cart = new CartBuilder().WithUserId("user1").WithItem().Build();
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync(cart);

        var result = await _sut.GetCartAsync("user1");

        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCartAsync_NoCartExists_ReturnsEmptyCart()
    {
        _cartRepoMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync((Cart?)null);

        var result = await _sut.GetCartAsync("user1");

        result.Items.Should().BeNullOrEmpty();
    }

    // ── UpdateItemAsync happy path ────────────────────────────────────────────

    [Fact]
    public async Task UpdateItemAsync_ValidRequest_UpdatesItemQuantity()
    {
        var cart = new CartBuilder().WithUserId("user1").WithItem(productId: 1, quantity: 2, stock: 20).Build();
        var item = cart.Items.First();
        var product = new ProductBuilder().WithId(1).WithStock(20).Build();

        _cartRepoMock.Setup(r => r.GetItemAsync(item.Id)).ReturnsAsync(item);
        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        var result = await _sut.UpdateItemAsync("user1", item.Id, new UpdateCartItemDto { Quantity = 5 });

        result.Items.First().Quantity.Should().Be(5);
    }

    // ── RemoveItemAsync happy path ────────────────────────────────────────────

    [Fact]
    public async Task RemoveItemAsync_OwnedItem_RemovesItemFromCart()
    {
        var cart = new CartBuilder().WithUserId("user1").WithItem().Build();
        var item = cart.Items.First();
        _cartRepoMock.Setup(r => r.GetItemAsync(item.Id)).ReturnsAsync(item);

        var result = await _sut.RemoveItemAsync("user1", item.Id);

        result.Items.Should().BeEmpty();
    }
}
