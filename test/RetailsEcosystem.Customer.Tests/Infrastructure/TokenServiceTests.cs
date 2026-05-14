using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Infrastructure.Identity;

namespace RetailsEcosystem.Customer.Tests.Infrastructure;

public class TokenServiceTests
{
    private const string ValidSecret = "super-secret-key-for-testing-purposes-1234567890abcdef!!";

    private static TokenService BuildService(
        string? secret = ValidSecret,
        string issuer = "TestIssuer",
        string audience = "TestAudience",
        int expiryMinutes = 15)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = secret,
                ["JwtSettings:Issuer"] = issuer,
                ["JwtSettings:Audience"] = audience,
                ["JwtSettings:AccessTokenExpiryMinutes"] = expiryMinutes.ToString()
            })
            .Build();
        return new TokenService(config);
    }

    private static ApplicationUser TestUser() => new()
    {
        Id = "user-abc-123",
        Email = "test@example.com",
        FullName = "Test User",
        UserName = "test@example.com",
        IsActive = true
    };

    // ── GenerateAccessToken ───────────────────────────────────────────────────

    [Fact]
    public void GenerateAccessToken_ContainsSubClaim_MatchingUserId()
    {
        var sut = BuildService();
        var user = TestUser();

        var token = sut.GenerateAccessToken(user, ["Customer"]);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Subject.Should().Be(user.Id);
    }

    [Fact]
    public void GenerateAccessToken_ContainsEmailClaim()
    {
        var sut = BuildService();
        var user = TestUser();

        var token = sut.GenerateAccessToken(user, []);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Should().Contain(c =>
            c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
    }

    [Fact]
    public void GenerateAccessToken_ContainsNameClaim()
    {
        var sut = BuildService();
        var user = TestUser();

        var token = sut.GenerateAccessToken(user, []);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Should().Contain(c =>
            c.Type == JwtRegisteredClaimNames.Name && c.Value == user.FullName);
    }

    [Fact]
    public void GenerateAccessToken_ContainsRoleClaimsForEachRole()
    {
        var sut = BuildService();

        var token = sut.GenerateAccessToken(TestUser(), ["Admin", "Customer"]);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        roles.Should().Contain("Admin").And.Contain("Customer");
    }

    [Fact]
    public void GenerateAccessToken_NoRoles_NoRoleClaims()
    {
        var sut = BuildService();

        var token = sut.GenerateAccessToken(TestUser(), []);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void GenerateAccessToken_ExpiryWithinTolerance()
    {
        var sut = BuildService(expiryMinutes: 30);
        var before = DateTime.UtcNow;

        var token = sut.GenerateAccessToken(TestUser(), []);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.ValidTo.Should().BeCloseTo(before.AddMinutes(30), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void GenerateAccessToken_ContainsUniqueJti()
    {
        var sut = BuildService();

        var t1 = sut.GenerateAccessToken(TestUser(), []);
        var t2 = sut.GenerateAccessToken(TestUser(), []);

        var j1 = new JwtSecurityTokenHandler().ReadJwtToken(t1);
        var j2 = new JwtSecurityTokenHandler().ReadJwtToken(t2);
        var jti1 = j1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var jti2 = j2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        jti1.Should().NotBe(jti2);
    }

    [Fact]
    public void GenerateAccessToken_MissingSecretKey_ThrowsInvalidOperationException()
    {
        var sut = BuildService(secret: null);

        var act = () => sut.GenerateAccessToken(TestUser(), []);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SecretKey*");
    }

    // ── GenerateRefreshToken ──────────────────────────────────────────────────

    [Fact]
    public void GenerateRefreshToken_Returns88CharBase64String()
    {
        var sut = BuildService();

        var token = sut.GenerateRefreshToken();

        token.Should().HaveLength(88);
        var bytes = Convert.FromBase64String(token);
        bytes.Should().HaveCount(64);
    }

    [Fact]
    public void GenerateRefreshToken_TwoCallsProduceDifferentTokens()
    {
        var sut = BuildService();

        var t1 = sut.GenerateRefreshToken();
        var t2 = sut.GenerateRefreshToken();

        t1.Should().NotBe(t2);
    }

    // ── GetPrincipalFromExpiredToken ──────────────────────────────────────────

    [Fact]
    public void GetPrincipalFromExpiredToken_ExpiredToken_ReturnsPrincipalWithClaims()
    {
        // TokenService cannot generate an already-expired token (notBefore < expires constraint).
        // Build one manually with the same key/settings.
        var sut = BuildService();
        var user = TestUser();
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(ValidSecret));
        var expiredJwt = new JwtSecurityToken(
            issuer: "TestIssuer",
            audience: "TestAudience",
            claims: [new Claim(JwtRegisteredClaimNames.Sub, user.Id), new Claim(JwtRegisteredClaimNames.Email, user.Email!)],
            notBefore: DateTime.UtcNow.AddMinutes(-30),
            expires: DateTime.UtcNow.AddMinutes(-15),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        var expiredTokenString = new JwtSecurityTokenHandler().WriteToken(expiredJwt);

        var principal = sut.GetPrincipalFromExpiredToken(expiredTokenString);

        principal.FindFirstValue(ClaimTypes.NameIdentifier).Should().Be(user.Id);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ValidToken_AlsoWorks()
    {
        var sut = BuildService(expiryMinutes: 15);
        var user = TestUser();
        var token = sut.GenerateAccessToken(user, []);

        var principal = sut.GetPrincipalFromExpiredToken(token);

        principal.FindFirstValue(ClaimTypes.NameIdentifier).Should().Be(user.Id);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WrongSigningKey_Throws()
    {
        var signer = BuildService(secret: ValidSecret);
        var verifier = BuildService(secret: "completely-different-key-for-testing-0987654321!!");
        var token = signer.GenerateAccessToken(TestUser(), []);

        var act = () => verifier.GetPrincipalFromExpiredToken(token);

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_Garbage_Throws()
    {
        var sut = BuildService();

        var act = () => sut.GetPrincipalFromExpiredToken("not.a.token");

        act.Should().Throw<Exception>();
    }

    [Fact]
    public void GenerateAccessToken_ProducesValidSignature()
    {
        var sut = BuildService();
        var token = sut.GenerateAccessToken(TestUser(), []);

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "TestIssuer",
            ValidAudience = "TestAudience",
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(ValidSecret))
        };

        var act = () => new JwtSecurityTokenHandler()
            .ValidateToken(token, validationParams, out _);

        act.Should().NotThrow();
    }
}
