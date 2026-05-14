using System.Net;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using RetailsEcosystem.Customer.Web.Controllers;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Tests.Web.Controllers;

public class CartControllerTests
{
    private readonly Mock<ICartService> _cartSvcMock = new();
    private readonly Mock<IProductService> _productSvcMock = new();

    private CartController BuildController(string accessToken = "tok")
    {
        var claims = new List<Claim>
        {
            new("access_token", accessToken),
            new(ClaimTypes.Name, "User"),
            new(ClaimTypes.Email, "a@b.com")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        var controller = new CartController(_cartSvcMock.Object, _productSvcMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    private static CartDto EmptyCart() => new() { Items = [] };

    private static CartDto CartWithItem(int qty = 2) => new()
    {
        Items = [new CartItemDto
        {
            Id = 1,
            ProductId = 10,
            ProductName = "Widget",
            Quantity = qty,
            UnitPrice = 5m
        }]
    };

    // ── GET /cart ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Index_NoStockWarnings_ReturnsViewWithCart()
    {
        var cart = CartWithItem(qty: 2);
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(cart);
        _productSvcMock.Setup(s => s.GetByIdAsync(10))
            .ReturnsAsync(new ProductDto { Id = 10, StockQuantity = 10 });
        var sut = BuildController();

        var result = await sut.Index();

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeEquivalentTo(cart);
        ((object?)sut.ViewBag.StockWarnings).Should().BeNull();
    }

    [Fact]
    public async Task Index_ItemExceedsStock_ViewBagContainsWarning()
    {
        var cart = CartWithItem(qty: 5);
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(cart);
        _productSvcMock.Setup(s => s.GetByIdAsync(10))
            .ReturnsAsync(new ProductDto { Id = 10, Name = "Widget", StockQuantity = 2 });
        var sut = BuildController();

        await sut.Index();

        var warnings = sut.ViewBag.StockWarnings as List<string>;
        warnings.Should().NotBeNullOrEmpty();
        warnings![0].Should().Contain("Widget");
    }

    [Fact]
    public async Task Index_EmptyCart_ReturnsViewWithoutWarnings()
    {
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(EmptyCart());
        var sut = BuildController();

        var result = await sut.Index();

        result.Should().BeOfType<ViewResult>();
        ((object?)sut.ViewBag.StockWarnings).Should().BeNull();
    }

    // ── POST /cart/items ──────────────────────────────────────────────────────

    [Fact]
    public async Task AddItem_ValidRequest_ReturnsOkWithItemCount()
    {
        _cartSvcMock.Setup(s => s.AddItemAsync("tok", 10, 1)).ReturnsAsync(CartWithItem());
        var sut = BuildController();

        var result = await sut.AddItem(new CartController.AddItemRequest(10, 1));

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
        json.Should().Contain("itemCount");
    }

    [Fact]
    public async Task AddItem_BadRequestFromApi_ReturnsBadRequest()
    {
        _cartSvcMock.Setup(s => s.AddItemAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new HttpRequestException("Out of stock.", null, HttpStatusCode.BadRequest));
        var sut = BuildController();

        var result = await sut.AddItem(new CartController.AddItemRequest(10, 100));

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddItem_NotFoundFromApi_ReturnsNotFound()
    {
        _cartSvcMock.Setup(s => s.AddItemAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new HttpRequestException("Not found.", null, HttpStatusCode.NotFound));
        var sut = BuildController();

        var result = await sut.AddItem(new CartController.AddItemRequest(99, 1));

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ── PUT /cart/items/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task UpdateItem_ValidRequest_ReturnsOkWithCart()
    {
        _cartSvcMock.Setup(s => s.UpdateItemAsync("tok", 1, 3)).ReturnsAsync(CartWithItem(qty: 3));
        var sut = BuildController();

        var result = await sut.UpdateItem(1, new CartController.UpdateItemRequest(3));

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<CartDto>();
    }

    [Fact]
    public async Task UpdateItem_BadRequestFromApi_ReturnsBadRequest()
    {
        _cartSvcMock.Setup(s => s.UpdateItemAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new HttpRequestException("Bad.", null, HttpStatusCode.BadRequest));
        var sut = BuildController();

        var result = await sut.UpdateItem(1, new CartController.UpdateItemRequest(999));

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── DELETE /cart/items/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task RemoveItem_ReturnsOkWithCart()
    {
        _cartSvcMock.Setup(s => s.RemoveItemAsync("tok", 1)).ReturnsAsync(EmptyCart());
        var sut = BuildController();

        var result = await sut.RemoveItem(1);

        result.Should().BeOfType<OkObjectResult>();
    }

    // ── POST /cart/clear ──────────────────────────────────────────────────────

    [Fact]
    public async Task Clear_RedirectsToIndex()
    {
        _cartSvcMock.Setup(s => s.ClearCartAsync("tok")).Returns(Task.CompletedTask);
        var sut = BuildController();

        var result = await sut.Clear();

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
    }

    // ── GET /cart/count ───────────────────────────────────────────────────────

    [Fact]
    public async Task Count_AnonymousUser_ReturnsZero()
    {
        var controller = new CartController(_cartSvcMock.Object, _productSvcMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        var result = await controller.Count();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
        json.Should().Contain("0");
    }

    [Fact]
    public async Task Count_AuthenticatedUser_ReturnsCartCount()
    {
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(CartWithItem());
        var sut = BuildController();

        var result = await sut.Count();

        result.Should().BeOfType<OkObjectResult>();
    }

    // ── GET /cart/summary ─────────────────────────────────────────────────────

    [Fact]
    public async Task Summary_AnonymousUser_ReturnsEmptyResult()
    {
        var controller = new CartController(_cartSvcMock.Object, _productSvcMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        var result = await controller.Summary();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
        json.Should().Contain("itemCount");
    }

    [Fact]
    public async Task Summary_AuthenticatedUser_ReturnsCartItems()
    {
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(CartWithItem());
        var sut = BuildController();

        var result = await sut.Summary();

        result.Should().BeOfType<OkObjectResult>();
    }
}
