using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace RetailsEcosystem.Customer.IntegrationTests.Auth;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    // Admin credentials seeded by IdentitySeed
    private const string AdminEmail = "admin@retailsecosystem.com";
    private const string AdminPassword = "Admin@123456";

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns200AndAccessToken()
    {
        var dto = new
        {
            FullName = "Integration Tester",
            Email = "integration_reg@example.com",
            Password = "Test@12345",
            ConfirmPassword = "Test@12345"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("accessToken");
    }

    [Fact]
    public async Task Register_WithInvalidEmail_Returns400()
    {
        var dto = new
        {
            FullName = "Bad User",
            Email = "not-an-email",
            Password = "Test@12345",
            ConfirmPassword = "Test@12345"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_Returns400()
    {
        var dto = new
        {
            FullName = "Bad User",
            Email = "mismatch@example.com",
            Password = "Test@12345",
            ConfirmPassword = "Different@99"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithAdminCredentials_Returns200AndAccessToken()
    {
        var dto = new { Email = AdminEmail, Password = AdminPassword };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("accessToken");
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var dto = new { Email = AdminEmail, Password = "WrongPassword@999" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyCredentials_Returns400()
    {
        var dto = new { Email = "", Password = "" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Me_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
