using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Web.Controllers;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Models.Vnpay;

namespace RetailsEcosystem.Customer.Tests.Web.Controllers;

public class VnpayControllerTests
{
    private readonly Mock<IVnpayWebService> _vnpaySvcMock  = new();
    private readonly Mock<IOrderService>    _orderSvcMock  = new();

    private VnpayController BuildController(
        bool authenticated  = false,
        string? accessToken = null,
        Dictionary<string, string>? queryParams = null)
    {
        var claims = new List<Claim>();
        if (accessToken != null) claims.Add(new Claim("access_token", accessToken));
        if (authenticated)
        {
            claims.Add(new Claim(ClaimTypes.Name, "User"));
            claims.Add(new Claim(ClaimTypes.Email, "a@b.com"));
        }

        var identity    = authenticated
            ? new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
            : new ClaimsIdentity(claims);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        if (queryParams is { Count: > 0 })
        {
            var queryString = "?" + string.Join("&",
                queryParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            httpContext.Request.QueryString = new QueryString(queryString);
        }

        var controller = new VnpayController(
            _vnpaySvcMock.Object,
            _orderSvcMock.Object,
            NullLogger<VnpayController>.Instance);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    private static Dictionary<string, string> ValidParams(string code = "00") =>
        new()
        {
            ["vnp_ResponseCode"] = code,
            ["vnp_TxnRef"]       = "1-abc",
            ["vnp_Amount"]       = "5000000",
            ["vnp_SecureHash"]   = "hash"
        };

    // ── Invalid signature ─────────────────────────────────────────────────────

    [Fact]
    public async Task Return_InvalidSignature_ReturnsFailureView()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(false);
        var sut = BuildController();

        var result = await sut.Return();

        var view = result.Should().BeOfType<ViewResult>().Subject;
        var model = view.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.Success.Should().BeFalse();
        model.Retryable.Should().BeFalse();
    }

    // ── Success (00) ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Return_ResponseCode00_SuccessTrue()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        var sut = BuildController(queryParams: ValidParams("00"));

        var result = await sut.Return();

        var model = result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.Success.Should().BeTrue();
        model.ResponseCode.Should().Be("00");
    }

    // ── Cancelled (24) ────────────────────────────────────────────────────────

    [Fact]
    public async Task Return_ResponseCode24_CancelledTrue()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        var sut = BuildController(queryParams: ValidParams("24"));

        var result = await sut.Return();

        var model = result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.Cancelled.Should().BeTrue();
    }

    // ── Unknown response code ─────────────────────────────────────────────────

    [Fact]
    public async Task Return_UnknownCode_MessageContainsCode()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        var sut = BuildController(queryParams: ValidParams("99"));

        var result = await sut.Return();

        var model = result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.Message.Should().Contain("99");
    }

    // ── Authenticated — confirms return ───────────────────────────────────────

    [Fact]
    public async Task Return_Authenticated_CallsConfirmReturn()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        _orderSvcMock.Setup(s => s.GetOrderByIdAsync("tok", 1))
            .ReturnsAsync(new OrderDto { Id = 1, PaymentAttemptCount = 1 });
        var sut = BuildController(authenticated: true, accessToken: "tok",
            queryParams: ValidParams("00"));

        await sut.Return();

        _orderSvcMock.Verify(s => s.ConfirmReturnAsync("tok", It.IsAny<Dictionary<string, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Return_Unauthenticated_SkipsConfirmReturn()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        var sut = BuildController(authenticated: false, queryParams: ValidParams("00"));

        await sut.Return();

        _orderSvcMock.Verify(s => s.ConfirmReturnAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()),
            Times.Never);
    }

    // ── ConfirmReturn throws — still returns view ─────────────────────────────

    [Fact]
    public async Task Return_ConfirmReturnThrows_StillReturnsView()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        _orderSvcMock.Setup(s => s.ConfirmReturnAsync("tok", It.IsAny<Dictionary<string, string>>()))
            .ThrowsAsync(new HttpRequestException("API down"));
        var sut = BuildController(authenticated: true, accessToken: "tok",
            queryParams: ValidParams("00"));

        var result = await sut.Return();

        result.Should().BeOfType<ViewResult>();
    }

    // ── GetOrderById throws — attemptsLeft = -1 ───────────────────────────────

    [Fact]
    public async Task Return_GetOrderThrows_AttemptsLeftIsMinusOne()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        _orderSvcMock.Setup(s => s.ConfirmReturnAsync("tok", It.IsAny<Dictionary<string, string>>()))
            .Returns(Task.CompletedTask);
        _orderSvcMock.Setup(s => s.GetOrderByIdAsync("tok", 1))
            .ThrowsAsync(new HttpRequestException("Not found"));
        var sut = BuildController(authenticated: true, accessToken: "tok",
            queryParams: ValidParams("07")); // failure code so retryable logic runs

        var result = await sut.Return();

        var model = result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.AttemptsLeft.Should().Be(-1);
    }

    // ── AttemptsLeft = 0 → Retryable false ───────────────────────────────────

    [Fact]
    public async Task Return_AttemptsExhausted_RetryableFalse()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        _orderSvcMock.Setup(s => s.ConfirmReturnAsync("tok", It.IsAny<Dictionary<string, string>>()))
            .Returns(Task.CompletedTask);
        _orderSvcMock.Setup(s => s.GetOrderByIdAsync("tok", 1))
            .ReturnsAsync(new OrderDto { Id = 1, PaymentAttemptCount = 3 });
        var sut = BuildController(authenticated: true, accessToken: "tok",
            queryParams: ValidParams("07")); // failure code

        var result = await sut.Return();

        var model = result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.Retryable.Should().BeFalse();
        model.AttemptsLeft.Should().Be(0);
    }

    // ── Response code message branches ───────────────────────────────────────

    [Theory]
    [InlineData("07", "fraud")]
    [InlineData("09", "online")]
    [InlineData("10", "three")]
    [InlineData("11", "expired")]
    [InlineData("12", "locked")]
    [InlineData("13", "OTP")]
    [InlineData("51", "balance")]
    [InlineData("65", "limit")]
    [InlineData("75", "maintenance")]
    [InlineData("79", "times")]
    public async Task Return_KnownResponseCode_MessageContainsExpectedText(string code, string expectedText)
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        var sut = BuildController(queryParams: ValidParams(code));

        var result = await sut.Return();

        var model = result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.Message.Should().Contain(expectedText, because: $"code {code} should produce message with '{expectedText}'");
    }

    // ── Authenticated but no access_token claim ───────────────────────────────

    [Fact]
    public async Task Return_AuthenticatedNoAccessToken_SkipsConfirmReturn()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        // authenticated=true but accessToken=null → no access_token claim
        var sut = BuildController(authenticated: true, accessToken: null,
            queryParams: ValidParams("00"));

        await sut.Return();

        _orderSvcMock.Verify(s => s.ConfirmReturnAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()),
            Times.Never);
    }

    // ── orderId = 0 (empty txnRef) → retryable false ─────────────────────────

    [Fact]
    public async Task Return_OrderIdZero_RetryableFalse()
    {
        _vnpaySvcMock.Setup(s => s.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(true);
        var sut = BuildController(queryParams: new Dictionary<string, string>
        {
            ["vnp_ResponseCode"] = "51",
            ["vnp_TxnRef"]       = "notanint-abc",
            ["vnp_Amount"]       = "5000000",
            ["vnp_SecureHash"]   = "hash"
        });

        var result = await sut.Return();

        var model = result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<VnpayReturnViewModel>().Subject;
        model.Retryable.Should().BeFalse();
    }
}
