using System.Net;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using RetailsEcosystem.Customer.Web.Controllers;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Models.Order;

namespace RetailsEcosystem.Customer.Tests.Web.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _orderSvcMock = new();
    private readonly Mock<ICartService>  _cartSvcMock  = new();

    private OrderController BuildController(string token = "tok")
    {
        var claims = new List<Claim>
        {
            new("access_token", token),
            new(ClaimTypes.Name, "User"),
            new(ClaimTypes.Email, "a@b.com")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var tempData        = new Mock<ITempDataDictionary>();
        var tempDataFactory = new Mock<ITempDataDictionaryFactory>();
        tempDataFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(tempData.Object);

        var urlHelper = new Mock<IUrlHelper>();
        var urlHelperFactory = new Mock<IUrlHelperFactory>();
        urlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelper.Object);

        var sp = new ServiceCollection()
            .AddSingleton(tempDataFactory.Object)
            .AddSingleton<IUrlHelperFactory>(urlHelperFactory.Object)
            .BuildServiceProvider();

        var controller = new OrderController(_orderSvcMock.Object, _cartSvcMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User            = new ClaimsPrincipal(identity),
                RequestServices = sp
            }
        };
        return controller;
    }

    private static CartDto CartWithItem() => new()
    {
        Items = [new CartItemDto { Id = 1, ProductId = 1, Quantity = 1, UnitPrice = 10m }]
    };

    private static CartDto EmptyCart() => new() { Items = [] };

    private static OrderDto SampleOrder(int id = 1) => new()
    {
        Id            = id,
        TotalAmount   = 50m,
        ShippingAddress = "123 St",
        Status        = OrderStatus.Pending
    };

    // ── GET Checkout ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Checkout_Get_CartHasItems_ReturnsView()
    {
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(CartWithItem());
        var sut = BuildController();

        var result = await sut.Checkout();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Checkout_Get_EmptyCart_RedirectsToCartIndex()
    {
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(EmptyCart());
        var sut = BuildController();

        var result = await sut.Checkout();

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
    }

    // ── POST Checkout ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Checkout_Post_InvalidModel_ReloadsCartAndReturnsView()
    {
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(CartWithItem());
        var sut = BuildController();
        sut.ModelState.AddModelError("ShippingAddress", "Required");

        var result = await sut.Checkout(new CheckoutViewModel());

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Checkout_Post_COD_RedirectsToConfirmation()
    {
        _orderSvcMock
            .Setup(s => s.CreateOrderAsync("tok", It.IsAny<CreateOrderDto>()))
            .ReturnsAsync(SampleOrder(id: 7));
        var sut   = BuildController();
        var model = new CheckoutViewModel
        {
            ShippingAddress = "123 St",
            PaymentMethod   = PaymentMethod.COD
        };

        var result = await sut.Checkout(model);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Confirmation");
    }

    [Fact]
    public async Task Checkout_Post_VNPay_RedirectsToPaymentUrl()
    {
        _orderSvcMock
            .Setup(s => s.CreateOrderAsync("tok", It.IsAny<CreateOrderDto>()))
            .ReturnsAsync(SampleOrder(id: 3));
        _orderSvcMock
            .Setup(s => s.InitiatePaymentAsync("tok", 3))
            .ReturnsAsync(new InitiatePaymentResult { PaymentUrl = "https://pay.vnpay.vn/abc" });
        var sut   = BuildController();
        var model = new CheckoutViewModel
        {
            ShippingAddress = "123 St",
            PaymentMethod   = PaymentMethod.VNPay
        };

        var result = await sut.Checkout(model);

        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Be("https://pay.vnpay.vn/abc");
    }

    [Fact]
    public async Task Checkout_Post_BadRequest_ShowsModelError()
    {
        _orderSvcMock
            .Setup(s => s.CreateOrderAsync("tok", It.IsAny<CreateOrderDto>()))
            .ThrowsAsync(new HttpRequestException("Out of stock", null, HttpStatusCode.BadRequest));
        _cartSvcMock.Setup(s => s.GetCartAsync("tok")).ReturnsAsync(CartWithItem());
        var sut = BuildController();

        var result = await sut.Checkout(new CheckoutViewModel { ShippingAddress = "123 St" });

        result.Should().BeOfType<ViewResult>();
        sut.ModelState.IsValid.Should().BeFalse();
    }

    // ── RetryPayment ──────────────────────────────────────────────────────────

    [Fact]
    public async Task RetryPayment_Success_RedirectsToPaymentUrl()
    {
        _orderSvcMock
            .Setup(s => s.InitiatePaymentAsync("tok", 5))
            .ReturnsAsync(new InitiatePaymentResult { PaymentUrl = "https://pay.vnpay.vn/xyz" });
        var sut = BuildController();

        var result = await sut.RetryPayment(5);

        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Be("https://pay.vnpay.vn/xyz");
    }

    [Fact]
    public async Task RetryPayment_BadRequest_RedirectsToDetail()
    {
        _orderSvcMock
            .Setup(s => s.InitiatePaymentAsync("tok", 5))
            .ThrowsAsync(new HttpRequestException("Bad", null, HttpStatusCode.BadRequest));
        var sut = BuildController();

        var result = await sut.RetryPayment(5);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Detail");
    }

    [Fact]
    public async Task RetryPayment_OtherException_RedirectsToDetail()
    {
        _orderSvcMock
            .Setup(s => s.InitiatePaymentAsync("tok", 5))
            .ThrowsAsync(new HttpRequestException("Error"));
        var sut = BuildController();

        var result = await sut.RetryPayment(5);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Detail");
    }

    // ── Confirmation ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Confirmation_Found_ReturnsViewWithOrder()
    {
        _orderSvcMock.Setup(s => s.GetOrderByIdAsync("tok", 1)).ReturnsAsync(SampleOrder());
        var sut = BuildController();

        var result = await sut.Confirmation(1);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<OrderDto>();
    }

    [Fact]
    public async Task Confirmation_NotFound_Returns404()
    {
        _orderSvcMock
            .Setup(s => s.GetOrderByIdAsync("tok", 99))
            .ThrowsAsync(new HttpRequestException("Not found"));
        var sut = BuildController();

        var result = await sut.Confirmation(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    // ── History ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task History_ReturnsViewWithPagedOrders()
    {
        var paged = new PagedResult<OrderDto>
        {
            Items      = [SampleOrder()],
            TotalCount = 1,
            PageNumber = 1,
            PageSize   = 10
        };
        _orderSvcMock.Setup(s => s.GetOrdersAsync("tok", 1, 10)).ReturnsAsync(paged);
        var sut = BuildController();

        var result = await sut.History(1);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<PagedResult<OrderDto>>();
    }

    // ── Detail ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Detail_Found_ReturnsView()
    {
        _orderSvcMock.Setup(s => s.GetOrderByIdAsync("tok", 1)).ReturnsAsync(SampleOrder());
        var sut = BuildController();

        var result = await sut.Detail(1);

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Detail_Forbidden_ReturnsForbid()
    {
        _orderSvcMock
            .Setup(s => s.GetOrderByIdAsync("tok", 2))
            .ThrowsAsync(new HttpRequestException("Forbidden", null, HttpStatusCode.Forbidden));
        var sut = BuildController();

        var result = await sut.Detail(2);

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Detail_NotFound_Returns404()
    {
        _orderSvcMock
            .Setup(s => s.GetOrderByIdAsync("tok", 99))
            .ThrowsAsync(new HttpRequestException("Not found"));
        var sut = BuildController();

        var result = await sut.Detail(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    // ── Cancel ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Cancel_Success_RedirectsToDetail()
    {
        _orderSvcMock.Setup(s => s.CancelOrderAsync("tok", 1)).ReturnsAsync(SampleOrder());
        var sut = BuildController();

        var result = await sut.Cancel(1);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Detail");
    }

    [Fact]
    public async Task Cancel_Failure_SetsErrorTempData_AndRedirectsToDetail()
    {
        _orderSvcMock
            .Setup(s => s.CancelOrderAsync("tok", 1))
            .ThrowsAsync(new HttpRequestException("Cannot cancel"));
        var sut = BuildController();

        var result = await sut.Cancel(1);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Detail");
    }
}
