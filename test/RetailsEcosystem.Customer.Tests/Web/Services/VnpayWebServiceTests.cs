using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Settings;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Tests.Web.Services;

public class VnpayWebServiceTests
{
    private const string HashSecret = "test-secret-key";

    private static VnpayWebService Build(string? hashSecret = HashSecret) =>
        new(Options.Create(new VnpaySettings
        {
            TmnCode    = "TESTCODE",
            HashSecret = hashSecret ?? HashSecret,
            ReturnUrl  = "https://test.local/vnpay/return",
            PaymentUrl = "https://pay.vnpay.vn/vpcpay.html",
            QueryDrUrl = "https://pay.vnpay.vn/querydr"
        }));

    private static OrderDto SampleOrder() => new()
    {
        Id          = 42,
        TotalAmount = 100m
    };

    // ── BuildPaymentUrl ───────────────────────────────────────────────────────

    [Fact]
    public void BuildPaymentUrl_ContainsRequiredVnpParams()
    {
        var svc = Build();

        var url = svc.BuildPaymentUrl(SampleOrder(), "42-abc", "127.0.0.1");

        url.Should().Contain("vnp_TmnCode=TESTCODE");
        url.Should().Contain("vnp_TxnRef=42-abc");
        url.Should().Contain("vnp_SecureHash=");
    }

    [Fact]
    public void BuildPaymentUrl_AmountIsMultipliedBy100()
    {
        var svc   = Build();
        var order = SampleOrder(); // TotalAmount = 100m

        var url = svc.BuildPaymentUrl(order, "42-abc", "127.0.0.1");

        // 100 * 100 = 10000
        url.Should().Contain("vnp_Amount=10000");
    }

    [Fact]
    public void BuildPaymentUrl_StartsWithPaymentBaseUrl()
    {
        var svc = Build();

        var url = svc.BuildPaymentUrl(SampleOrder(), "42-abc", "127.0.0.1");

        url.Should().StartWith("https://pay.vnpay.vn/vpcpay.html");
    }

    [Fact]
    public void BuildPaymentUrl_ContainsReturnUrl()
    {
        var svc = Build();

        var url = svc.BuildPaymentUrl(SampleOrder(), "42-abc", "127.0.0.1");

        url.Should().Contain("vnp_ReturnUrl=");
    }

    // ── ValidateSignature ─────────────────────────────────────────────────────

    [Fact]
    public void ValidateSignature_CorrectHash_ReturnsTrue()
    {
        var svc = Build();
        // Build a parameter set with a correctly-computed hash
        var data = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Amount"]       = "10000",
            ["vnp_ResponseCode"] = "00",
            ["vnp_TxnRef"]       = "42-abc"
        };
        var hashData = string.Join("&",
            data.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
        var hash = ComputeHmac(HashSecret, hashData);
        data["vnp_SecureHash"] = hash;

        svc.ValidateSignature(data).Should().BeTrue();
    }

    [Fact]
    public void ValidateSignature_WrongHash_ReturnsFalse()
    {
        var svc = Build();
        var data = new Dictionary<string, string>
        {
            ["vnp_Amount"]       = "10000",
            ["vnp_ResponseCode"] = "00",
            ["vnp_SecureHash"]   = "bad-hash"
        };

        svc.ValidateSignature(data).Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_MissingSecureHash_ReturnsFalse()
    {
        var svc  = Build();
        var data = new Dictionary<string, string> { ["vnp_Amount"] = "10000" };

        svc.ValidateSignature(data).Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_SecureHashExcludedFromHashData()
    {
        var svc = Build();
        // If vnp_SecureHash were included in hash data the signature would differ
        var data = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_ResponseCode"] = "00"
        };
        var hashData = string.Join("&",
            data.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
        var hash = ComputeHmac(HashSecret, hashData);
        data["vnp_SecureHash"] = hash;

        // Passing vnp_SecureHashType alongside vnp_SecureHash should still work
        data["vnp_SecureHashType"] = "HMACSHA512";

        svc.ValidateSignature(data).Should().BeTrue();
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    private static string ComputeHmac(string key, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLower();
    }
}
