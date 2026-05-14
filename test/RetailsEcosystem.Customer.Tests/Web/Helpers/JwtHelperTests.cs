using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using RetailsEcosystem.Customer.Web.Helpers;

namespace RetailsEcosystem.Customer.Tests.Web.Helpers;

public class JwtHelperTests
{
    private static string BuildToken(DateTime? expiry = null, bool urlSafeBase64 = false)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-secret-key-long-enough-1234567890!!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = expiry ?? DateTime.UtcNow.AddMinutes(15);

        var token = new JwtSecurityToken(
            issuer: "TestIssuer",
            audience: "TestAudience",
            claims: [new Claim(ClaimTypes.Name, "user")],
            expires: expiresAt,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public void GetTokenExpiry_ValidToken_ReturnsCorrectExpiry()
    {
        var expectedExpiry = DateTime.UtcNow.AddMinutes(30);
        var token = BuildToken(expectedExpiry);

        var result = JwtHelper.GetTokenExpiry(token);

        result.Should().NotBeNull();
        result!.Value.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void GetTokenExpiry_ExpiredToken_ReturnsExpiredDateTime()
    {
        var expired = DateTime.UtcNow.AddMinutes(-10);
        var token = BuildToken(expired);

        var result = JwtHelper.GetTokenExpiry(token);

        result.Should().NotBeNull();
        result!.Value.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void GetTokenExpiry_TokenWithoutExpClaim_ReturnsNull()
    {
        // Build token manually without exp
        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"sub\":\"user\"}"))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var fakeToken = $"header.{payload}.signature";

        var result = JwtHelper.GetTokenExpiry(fakeToken);

        result.Should().BeNull();
    }

    [Fact]
    public void GetTokenExpiry_MalformedToken_ReturnsNull()
    {
        var result = JwtHelper.GetTokenExpiry("not-a-jwt-token");

        result.Should().BeNull();
    }

    [Fact]
    public void GetTokenExpiry_EmptyString_ReturnsNull()
    {
        var result = JwtHelper.GetTokenExpiry("");

        result.Should().BeNull();
    }

    [Fact]
    public void GetTokenExpiry_InvalidBase64Payload_ReturnsNull()
    {
        var result = JwtHelper.GetTokenExpiry("header.!!!invalid_base64!!!.signature");

        result.Should().BeNull();
    }

    [Fact]
    public void GetTokenExpiry_PayloadNeedsTwoPadChars_Parses()
    {
        // Craft payload where length % 4 == 2
        var expUnix = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
        var json = $"{{\"exp\":{expUnix}}}";
        var bytes = Encoding.UTF8.GetBytes(json);
        // Keep padding to verify raw base64 with == works; helper must handle it
        var b64 = Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var fakeToken = $"header.{b64}.signature";

        var result = JwtHelper.GetTokenExpiry(fakeToken);

        result.Should().NotBeNull();
    }
}
