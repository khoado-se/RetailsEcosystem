using System.Net;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Settings;

namespace RetailsEcosystem.Customer.Tests.Services;

public class VnpayServiceTests
{
    private const string TestTmnCode    = "TEST_TMN";
    private const string TestHashSecret = "test-secret-key-for-unit-tests";

    private static VnpaySettings TestSettings => new()
    {
        TmnCode    = TestTmnCode,
        HashSecret = TestHashSecret,
        PaymentUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
        ReturnUrl  = "https://localhost/vnpay/return",
        QueryDrUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction",
    };

    private static VnpayService BuildSut()
    {
        var options     = Options.Create(TestSettings);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient("VNPay")).Returns(new HttpClient());
        return new VnpayService(options, factoryMock.Object);
    }

    private static string ComputeHmac(string secret, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
    }

    private static Dictionary<string, string> BuildSignedParams(Dictionary<string, string> raw, string secret)
    {
        var sorted   = new SortedDictionary<string, string>(raw, StringComparer.Ordinal);
        var hashData = string.Join("&", sorted.Select(kv =>
            $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
        var hash = ComputeHmac(secret, hashData);
        raw["vnp_SecureHash"] = hash;
        return raw;
    }

    // ── ValidateSignature ────────────────────────────────────────────────────

    [Fact]
    public void ValidateSignature_CorrectHmac_ReturnsTrue()
    {
        var sut    = BuildSut();
        var @params = BuildSignedParams(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]       = "1-20260101120000",
            ["vnp_ResponseCode"] = "00",
            ["vnp_Amount"]       = "5000000",
        }, TestHashSecret);

        sut.ValidateSignature(@params).Should().BeTrue();
    }

    [Fact]
    public void ValidateSignature_TamperedParam_ReturnsFalse()
    {
        var sut    = BuildSut();
        var @params = BuildSignedParams(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]       = "1-20260101120000",
            ["vnp_ResponseCode"] = "00",
            ["vnp_Amount"]       = "5000000",
        }, TestHashSecret);

        @params["vnp_Amount"] = "9999999"; // tamper after signing

        sut.ValidateSignature(@params).Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_MissingSecureHash_ReturnsFalse()
    {
        var sut = BuildSut();
        var @params = new Dictionary<string, string>
        {
            ["vnp_TxnRef"]       = "1-20260101120000",
            ["vnp_ResponseCode"] = "00",
        };

        sut.ValidateSignature(@params).Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_SecureHashTypeKeyPresent_StillValidates()
    {
        var sut    = BuildSut();
        var @params = BuildSignedParams(new Dictionary<string, string>
        {
            ["vnp_TxnRef"]       = "1-20260101120000",
            ["vnp_ResponseCode"] = "00",
        }, TestHashSecret);

        @params["vnp_SecureHashType"] = "HmacSHA512"; // must be excluded from hash input

        sut.ValidateSignature(@params).Should().BeTrue();
    }

    // ── BuildPaymentUrl ──────────────────────────────────────────────────────

    [Fact]
    public void BuildPaymentUrl_AmountIsMultipliedBy100()
    {
        var sut   = BuildSut();
        var order = new OrderDto { Id = 1, TotalAmount = 150_000m };

        var url = sut.BuildPaymentUrl(order, "1-20260101", "127.0.0.1");

        url.Should().Contain("vnp_Amount=15000000");
    }

    [Fact]
    public void BuildPaymentUrl_TxnRefIncludedInUrl()
    {
        var sut   = BuildSut();
        var order = new OrderDto { Id = 2, TotalAmount = 50_000m };

        var url = sut.BuildPaymentUrl(order, "2-20260101120000", "127.0.0.1");

        url.Should().Contain("2-20260101120000");
    }

    [Fact]
    public void BuildPaymentUrl_TmnCodeIncludedInUrl()
    {
        var sut   = BuildSut();
        var order = new OrderDto { Id = 3, TotalAmount = 10_000m };

        var url = sut.BuildPaymentUrl(order, "3-20260101", "127.0.0.1");

        url.Should().Contain(TestTmnCode);
    }
}
