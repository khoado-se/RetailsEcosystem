using System.Net;
using System.Net.Http;
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

    // ── QueryTransactionAsync ────────────────────────────────────────────────

    private sealed class FakeHttpHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public FakeHttpHandler(HttpResponseMessage response) => _response = response;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(_response);
    }

    private static VnpayService BuildSutWithHttp(HttpResponseMessage response)
    {
        var options = Options.Create(TestSettings);
        var client  = new HttpClient(new FakeHttpHandler(response));
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("VNPay")).Returns(client);
        return new VnpayService(options, factory.Object);
    }

    [Fact]
    public async Task QueryTransactionAsync_SuccessResponse_ReturnsPopulatedResult()
    {
        var json = """{"vnp_ResponseCode":"00","vnp_TransactionStatus":"00","vnp_TransactionNo":"12345","vnp_Amount":"5000000","vnp_Message":"Success"}""";
        var sut  = BuildSutWithHttp(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });

        var result = await sut.QueryTransactionAsync("1-20260101", "20260101120000", "127.0.0.1");

        result.ResponseCode.Should().Be("00");
        result.TransactionNo.Should().Be("12345");
        result.Amount.Should().Be(5_000_000);
    }

    [Fact]
    public async Task QueryTransactionAsync_HttpFailure_ReturnsResponseCode99()
    {
        var sut = BuildSutWithHttp(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await sut.QueryTransactionAsync("1-20260101", "20260101120000", "127.0.0.1");

        result.ResponseCode.Should().Be("99");
        result.Message.Should().Be("Query failed");
    }

    [Fact]
    public async Task QueryTransactionAsync_AmountNotParseable_ReturnsZeroAmount()
    {
        var json = """{"vnp_ResponseCode":"00","vnp_Amount":"not-a-number"}""";
        var sut  = BuildSutWithHttp(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });

        var result = await sut.QueryTransactionAsync("1-20260101", "20260101120000", "127.0.0.1");

        result.ResponseCode.Should().Be("00");
        result.Amount.Should().Be(0);
    }

    [Fact]
    public async Task QueryTransactionAsync_AllFieldsAbsent_ReturnsDefaults()
    {
        var json = """{}""";
        var sut  = BuildSutWithHttp(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });

        var result = await sut.QueryTransactionAsync("1-20260101", "20260101120000", "127.0.0.1");

        result.ResponseCode.Should().BeEmpty();
        result.TransactionStatus.Should().BeEmpty();
        result.TransactionNo.Should().BeEmpty();
        result.Amount.Should().Be(0);
        result.Message.Should().BeEmpty();
    }

    [Fact]
    public async Task QueryTransactionAsync_MissingFields_ReturnsEmptyStrings()
    {
        var json = """{"vnp_ResponseCode":"01"}""";
        var sut  = BuildSutWithHttp(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        });

        var result = await sut.QueryTransactionAsync("1-20260101", "20260101120000", "127.0.0.1");

        result.ResponseCode.Should().Be("01");
        result.TransactionStatus.Should().BeEmpty();
        result.TransactionNo.Should().BeEmpty();
        result.Amount.Should().Be(0);
    }
}
