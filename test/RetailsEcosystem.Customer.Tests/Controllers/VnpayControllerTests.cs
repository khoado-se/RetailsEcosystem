using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class VnpayControllerTests
{
    private readonly Mock<IVnpayService>            _vnpayServiceMock = new();
    private readonly Mock<IOrderService>            _orderServiceMock = new();
    private readonly Mock<ILogger<VnpayController>> _loggerMock       = new();
    private readonly VnpayController _sut;

    public VnpayControllerTests()
    {
        _sut = new VnpayController(_vnpayServiceMock.Object, _orderServiceMock.Object, _loggerMock.Object);
        SetAuthenticatedUser("user-123");
    }

    private void SetAuthenticatedUser(string userId)
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, userId)
                ], "TestAuth"))
            }
        };
    }

    private void SetIpnQueryString(Dictionary<string, string> parameters)
    {
        var qs = string.Join("&", parameters.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        _sut.ControllerContext.HttpContext.Request.QueryString = new QueryString($"?{qs}");
    }

    // ── IPN — POST /api/vnpay/ipn ────────────────────────────────────────────

    [Fact]
    public async Task Ipn_InvalidSignature_ReturnsRspCode97()
    {
        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>()))
            .Returns(false);
        SetIpnQueryString(new() { ["vnp_TxnRef"] = "1-20260101" });

        var result = await _sut.Ipn();

        var ok    = result.Should().BeOfType<OkObjectResult>().Subject;
        var body  = ok.Value!.ToString()!;
        body.Should().Contain("97");
    }

    [Fact]
    public async Task Ipn_TxnRefNotFound_ReturnsRspCode01()
    {
        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync(It.IsAny<string>()))
            .ReturnsAsync((PaymentAttempt?)null);

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]       = "1-20260101",
            ["vnp_ResponseCode"] = "00",
        });

        var result = await _sut.Ipn();

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value!.ToString()!;
        body.Should().Contain("01");
    }

    [Fact]
    public async Task Ipn_AttemptAlreadyResolved_ReturnsAlreadyProcessed()
    {
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Succeeded };

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]       = "1-20260101",
            ["vnp_ResponseCode"] = "00",
        });

        var result = await _sut.Ipn();

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value!.ToString()!;
        body.Should().Contain("Already processed");
    }

    [Fact]
    public async Task Ipn_InvalidTxnRefFormat_ReturnsRspCode01()
    {
        var attempt = new PaymentAttempt { TxnRef = "badformat", Status = PaymentAttemptStatus.Initiated };

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync("badformat")).ReturnsAsync(attempt);

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]       = "badformat",
            ["vnp_ResponseCode"] = "00",
            ["vnp_Amount"]       = "5000000",
        });

        var result = await _sut.Ipn();

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value!.ToString()!;
        body.Should().Contain("01");
    }

    [Fact]
    public async Task Ipn_AmountMismatch_ReturnsRspCode04()
    {
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };
        var order   = new OrderBuilder().WithTotalAmount(500m).Build(); // 500 * 100 = 50000 expected

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, string.Empty, "Admin")).ReturnsAsync(MapToDto(order));

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]              = "1-20260101",
            ["vnp_ResponseCode"]        = "00",
            ["vnp_TransactionStatus"]   = "00",
            ["vnp_Amount"]              = "9999999", // wrong amount
            ["vnp_TransactionNo"]       = "vnp-001",
        });

        var result = await _sut.Ipn();

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value!.ToString()!;
        body.Should().Contain("04");
    }

    [Fact]
    public async Task Ipn_SuccessfulPayment_CallsConfirmVnpayPayment()
    {
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };
        var order   = new OrderBuilder().WithTotalAmount(500m).Build();

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, string.Empty, "Admin")).ReturnsAsync(MapToDto(order));

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]            = "1-20260101",
            ["vnp_ResponseCode"]      = "00",
            ["vnp_TransactionStatus"] = "00",
            ["vnp_Amount"]            = "50000",  // 500 * 100
            ["vnp_TransactionNo"]     = "vnp-001",
        });

        await _sut.Ipn();

        _orderServiceMock.Verify(
            s => s.ConfirmVnpayPaymentAsync(1, "vnp-001", "1-20260101"),
            Times.Once);
    }

    [Fact]
    public async Task Ipn_CancelledByUser_CallsRecordOutcomeWithCancelled()
    {
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };
        var order   = new OrderBuilder().WithTotalAmount(500m).Build();

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, string.Empty, "Admin")).ReturnsAsync(MapToDto(order));

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]            = "1-20260101",
            ["vnp_ResponseCode"]      = "24",
            ["vnp_TransactionStatus"] = "24",
            ["vnp_Amount"]            = "50000",
            ["vnp_TransactionNo"]     = "",
        });

        await _sut.Ipn();

        _orderServiceMock.Verify(
            s => s.RecordPaymentOutcomeAsync(1, "1-20260101", "24", PaymentAttemptStatus.Cancelled),
            Times.Once);
    }

    [Fact]
    public async Task Ipn_FailedPayment_CallsRecordOutcomeWithFailed()
    {
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };
        var order   = new OrderBuilder().WithTotalAmount(500m).Build();

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, string.Empty, "Admin")).ReturnsAsync(MapToDto(order));

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]            = "1-20260101",
            ["vnp_ResponseCode"]      = "51",
            ["vnp_TransactionStatus"] = "02",
            ["vnp_Amount"]            = "50000",
            ["vnp_TransactionNo"]     = "",
        });

        await _sut.Ipn();

        _orderServiceMock.Verify(
            s => s.RecordPaymentOutcomeAsync(1, "1-20260101", "51", PaymentAttemptStatus.Failed),
            Times.Once);
    }

    [Fact]
    public async Task Ipn_ConcurrencyException_ReturnsRspCode00()
    {
        var attempt = new PaymentAttempt { TxnRef = "1-20260101", Status = PaymentAttemptStatus.Initiated };
        var order   = new OrderBuilder().WithTotalAmount(500m).Build();

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetPaymentAttemptByTxnRefAsync("1-20260101")).ReturnsAsync(attempt);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, string.Empty, "Admin")).ReturnsAsync(MapToDto(order));
        _orderServiceMock
            .Setup(s => s.ConfirmVnpayPaymentAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new DbUpdateConcurrencyException());

        SetIpnQueryString(new()
        {
            ["vnp_TxnRef"]            = "1-20260101",
            ["vnp_ResponseCode"]      = "00",
            ["vnp_TransactionStatus"] = "00",
            ["vnp_Amount"]            = "50000",
            ["vnp_TransactionNo"]     = "vnp-001",
        });

        var result = await _sut.Ipn();

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value!.ToString()!;
        body.Should().Contain("00");
    }

    // ── ConfirmReturn — POST /api/vnpay/confirm-return ───────────────────────

    [Fact]
    public async Task ConfirmReturn_InvalidSignature_ReturnsBadRequest()
    {
        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(false);

        var result = await _sut.ConfirmReturn(new Dictionary<string, string> { ["vnp_TxnRef"] = "1-20260101" });

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ConfirmReturn_InvalidTxnRefFormat_ReturnsBadRequest()
    {
        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);

        var result = await _sut.ConfirmReturn(new Dictionary<string, string> { ["vnp_TxnRef"] = "badformat" });

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ConfirmReturn_OrderBelongsToDifferentUser_ReturnsForbid()
    {
        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock
            .Setup(s => s.GetOrderByIdAsync(1, "user-123", "Customer"))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _sut.ConfirmReturn(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]       = "1-20260101",
            ["vnp_ResponseCode"] = "00",
        });

        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task ConfirmReturn_OrderNotFound_ReturnsNotFound()
    {
        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock
            .Setup(s => s.GetOrderByIdAsync(1, "user-123", "Customer"))
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _sut.ConfirmReturn(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]       = "1-20260101",
            ["vnp_ResponseCode"] = "00",
        });

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ConfirmReturn_SuccessfulPayment_CallsConfirmAndReturnsOk()
    {
        var orderDto = new OrderDto { Id = 1, PaymentStatus = PaymentStatus.Paid };

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, "user-123", "Customer")).ReturnsAsync(orderDto);

        var result = await _sut.ConfirmReturn(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]            = "1-20260101",
            ["vnp_ResponseCode"]      = "00",
            ["vnp_TransactionStatus"] = "00",
            ["vnp_TransactionNo"]     = "vnp-001",
        });

        result.Should().BeOfType<OkObjectResult>();
        _orderServiceMock.Verify(
            s => s.ConfirmVnpayPaymentAsync(1, "vnp-001", "1-20260101"),
            Times.Once);
    }

    [Fact]
    public async Task ConfirmReturn_CancelledPayment_CallsRecordOutcomeWithCancelled()
    {
        var orderDto = new OrderDto { Id = 1, PaymentStatus = PaymentStatus.Cancelled };

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, "user-123", "Customer")).ReturnsAsync(orderDto);

        var result = await _sut.ConfirmReturn(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]            = "1-20260101",
            ["vnp_ResponseCode"]      = "24",
            ["vnp_TransactionStatus"] = "24",
            ["vnp_TransactionNo"]     = "",
        });

        result.Should().BeOfType<OkObjectResult>();
        _orderServiceMock.Verify(
            s => s.RecordPaymentOutcomeAsync(1, "1-20260101", "24", PaymentAttemptStatus.Cancelled),
            Times.Once);
    }

    [Fact]
    public async Task ConfirmReturn_FailedPayment_CallsRecordOutcomeWithFailed()
    {
        var orderDto = new OrderDto { Id = 1, PaymentStatus = PaymentStatus.Failed };

        _vnpayServiceMock.Setup(v => v.ValidateSignature(It.IsAny<IDictionary<string, string>>())).Returns(true);
        _orderServiceMock.Setup(s => s.GetOrderByIdAsync(1, "user-123", "Customer")).ReturnsAsync(orderDto);

        var result = await _sut.ConfirmReturn(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]            = "1-20260101",
            ["vnp_ResponseCode"]      = "11",
            ["vnp_TransactionStatus"] = "02",
            ["vnp_TransactionNo"]     = "",
        });

        result.Should().BeOfType<OkObjectResult>();
        _orderServiceMock.Verify(
            s => s.RecordPaymentOutcomeAsync(1, "1-20260101", "11", PaymentAttemptStatus.Failed),
            Times.Once);
    }

    private static OrderDto MapToDto(Domain.Entities.Order order) => new()
    {
        Id          = order.Id,
        UserId      = order.UserId,
        TotalAmount = order.TotalAmount,
        Status      = order.Status,
    };
}
