using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Tests.Web.Services;

public class AccountServiceTests
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static AccountService BuildService(
        HttpStatusCode status,
        object? body,
        Dictionary<string, string>? responseHeaders = null)
    {
        var json = body is string s ? s : JsonSerializer.Serialize(body, JsonOpts);
        var handler = new FakeHttpHandler(status, json, responseHeaders);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("MyApi")).Returns(client);
        return new AccountService(factory.Object);
    }

    private static AuthResponseDto ValidAuthResponse() => new()
    {
        AccessToken = "access-token",
        ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        User = new UserInfoDto { Id = "u1", Email = "a@b.com", FullName = "User" }
    };

    // ── LoginAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_200_ReturnsAuthResponse()
    {
        var svc = BuildService(HttpStatusCode.OK, ValidAuthResponse());

        var (auth, _) = await svc.LoginAsync("a@b.com", "pass");

        auth.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Login_401_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.Unauthorized,
            new { detail = "Invalid email or password." });

        var act = () => svc.LoginAsync("x@x.com", "wrong");

        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*Invalid email*");
    }

    [Fact]
    public async Task Login_401_NonJsonBody_ThrowsWithFallbackMessage()
    {
        var svc = BuildService(HttpStatusCode.Unauthorized, "plain text");

        var act = () => svc.LoginAsync("x@x.com", "wrong");

        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("*Invalid email or password*");
    }

    // ── RegisterAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Register_200_ReturnsAuthResponse()
    {
        var svc = BuildService(HttpStatusCode.OK, ValidAuthResponse());

        var (auth, _) = await svc.RegisterAsync("User", "a@b.com", "Pass1!", "Pass1!");

        auth.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Register_400_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.BadRequest, new { errors = new[] { "Email taken" } });

        var act = () => svc.RegisterAsync("User", "a@b.com", "Pass1!", "Pass1!");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── RefreshAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_200_ReturnsNewTokenPair()
    {
        var svc = BuildService(HttpStatusCode.OK, ValidAuthResponse());

        var (auth, _) = await svc.RefreshAsync("old-refresh");

        auth.Should().NotBeNull();
        auth!.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Refresh_401_ReturnsNullTuple()
    {
        var svc = BuildService(HttpStatusCode.Unauthorized, "{}");

        var (auth, refreshToken) = await svc.RefreshAsync("bad-token");

        auth.Should().BeNull();
        refreshToken.Should().BeNull();
    }

    // ── LogoutAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Logout_200_DoesNotThrow()
    {
        var svc = BuildService(HttpStatusCode.NoContent, "");

        var act = () => svc.LogoutAsync("access-token");

        await act.Should().NotThrowAsync();
    }

    // ── GetProfileAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetProfile_200_ReturnsCustomerDto()
    {
        var dto = new CustomerDto { FullName = "Test User", Email = "a@b.com" };
        var svc = BuildService(HttpStatusCode.OK, dto);

        var result = await svc.GetProfileAsync("access-token");

        result.FullName.Should().Be("Test User");
    }

    [Fact]
    public async Task GetProfile_401_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.Unauthorized, "{}");

        var act = () => svc.GetProfileAsync("bad-token");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── UpdateProfileAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateProfile_200_DoesNotThrow()
    {
        var svc = BuildService(HttpStatusCode.OK, "{}");

        var act = () => svc.UpdateProfileAsync("access-token",
            new UpdateProfileDto { FullName = "New Name" });

        await act.Should().NotThrowAsync();
    }

    // ── ChangePasswordAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ChangePassword_200_DoesNotThrow()
    {
        var svc = BuildService(HttpStatusCode.OK, "{}");

        var act = () => svc.ChangePasswordAsync("access-token", "old", "new");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ChangePassword_400_ThrowsHttpRequestException()
    {
        var svc = BuildService(HttpStatusCode.BadRequest, "{}");

        var act = () => svc.ChangePasswordAsync("access-token", "wrong-old", "new");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // ── ExtractRefreshToken from Set-Cookie ───────────────────────────────────

    [Fact]
    public async Task Login_SetCookieHeader_ExtractsRefreshToken()
    {
        var svc = BuildService(
            HttpStatusCode.OK,
            ValidAuthResponse(),
            responseHeaders: new Dictionary<string, string>
            {
                ["Set-Cookie"] = "refreshToken=my-secret-refresh; HttpOnly; Path=/"
            });

        var (_, refreshToken) = await svc.LoginAsync("a@b.com", "pass");

        refreshToken.Should().Be("my-secret-refresh");
    }

    [Fact]
    public async Task Login_NoSetCookieHeader_RefreshTokenIsNull()
    {
        var svc = BuildService(HttpStatusCode.OK, ValidAuthResponse());

        var (_, refreshToken) = await svc.LoginAsync("a@b.com", "pass");

        refreshToken.Should().BeNull();
    }

    private sealed class FakeHttpHandler(
        HttpStatusCode status,
        string body,
        Dictionary<string, string>? extraHeaders = null) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            if (extraHeaders != null)
            {
                foreach (var (k, v) in extraHeaders)
                    response.Headers.TryAddWithoutValidation(k, v);
            }
            return Task.FromResult(response);
        }
    }
}
