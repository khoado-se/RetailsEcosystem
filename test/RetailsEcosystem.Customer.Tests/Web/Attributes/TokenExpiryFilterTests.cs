using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using RetailsEcosystem.Customer.Web.Attributes;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Tests.Web.Attributes;

public class TokenExpiryFilterTests
{
    private static (ActionExecutingContext context, Mock<IAuthenticationService> authSvcMock)
        BuildContext(
            bool authenticated,
            string? expiresAt = null,
            string? refreshToken = null,
            string controllerName = "Products")
    {
        var claims = new List<Claim>();
        if (expiresAt != null)
            claims.Add(new Claim("token_expires_at", expiresAt));
        if (refreshToken != null)
            claims.Add(new Claim("refresh_token", refreshToken));

        var identity = authenticated
            ? new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
            : new ClaimsIdentity(claims);

        var authSvcMock = new Mock<IAuthenticationService>();
        authSvcMock
            .Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string?>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);
        authSvcMock
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string?>(),
                It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(authSvcMock.Object);
        var sp = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity),
            RequestServices = sp
        };
        httpContext.Request.Path = "/products";

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = controllerName,
            ActionName = "Index"
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            actionDescriptor);

        var context = new ActionExecutingContext(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new object());

        return (context, authSvcMock);
    }

    private static ActionExecutionDelegate BuildDelegate(bool throws = false)
    {
        if (throws)
            return () => throw new Exception("downstream error");

        return () => Task.FromResult(new ActionExecutedContext(
            new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ControllerActionDescriptor()),
            [],
            new object()));
    }

    private static IAccountService BuildAccountService(AuthResponseDto? response, string? newRefreshToken)
    {
        var mock = new Mock<IAccountService>();
        mock.Setup(s => s.RefreshAsync(It.IsAny<string>()))
            .ReturnsAsync((response, newRefreshToken));
        return mock.Object;
    }

    // ── Passthrough cases ─────────────────────────────────────────────────────

    [Fact]
    public async Task OnActionExecution_UnauthenticatedUser_PassesThrough()
    {
        var filter = new TokenExpiryFilter();
        var (ctx, _) = BuildContext(authenticated: false);
        var delegateCalled = false;
        ActionExecutionDelegate next = () =>
        {
            delegateCalled = true;
            return BuildDelegate()();
        };

        await filter.OnActionExecutionAsync(ctx, next);

        delegateCalled.Should().BeTrue();
        ctx.Result.Should().BeNull();
    }

    [Fact]
    public async Task OnActionExecution_AccountController_SkipsRefreshLogic()
    {
        var filter = new TokenExpiryFilter();
        var expiredAt = DateTime.UtcNow.AddMinutes(-5).ToString("O");
        var (ctx, _) = BuildContext(
            authenticated: true,
            expiresAt: expiredAt,
            controllerName: "Account");
        var delegateCalled = false;
        ActionExecutionDelegate next = () =>
        {
            delegateCalled = true;
            return BuildDelegate()();
        };

        await filter.OnActionExecutionAsync(ctx, next);

        delegateCalled.Should().BeTrue();
        ctx.Result.Should().BeNull();
    }

    [Fact]
    public async Task OnActionExecution_NoExpiryClaimPresent_PassesThrough()
    {
        var filter = new TokenExpiryFilter();
        var (ctx, _) = BuildContext(authenticated: true, expiresAt: null);
        var delegateCalled = false;
        ActionExecutionDelegate next = () =>
        {
            delegateCalled = true;
            return BuildDelegate()();
        };

        await filter.OnActionExecutionAsync(ctx, next);

        delegateCalled.Should().BeTrue();
    }

    [Fact]
    public async Task OnActionExecution_ValidToken_PassesThrough()
    {
        var filter = new TokenExpiryFilter();
        var futureExpiry = DateTime.UtcNow.AddMinutes(10).ToString("O");
        var (ctx, _) = BuildContext(authenticated: true, expiresAt: futureExpiry);
        var delegateCalled = false;
        ActionExecutionDelegate next = () =>
        {
            delegateCalled = true;
            return BuildDelegate()();
        };

        await filter.OnActionExecutionAsync(ctx, next);

        delegateCalled.Should().BeTrue();
        ctx.Result.Should().BeNull();
    }

    // ── Expired token — refresh succeeds ─────────────────────────────────────

    [Fact]
    public async Task OnActionExecution_ExpiredToken_WithValidRefreshToken_RefreshesAndContinues()
    {
        var filter = new TokenExpiryFilter();
        var expiredAt = DateTime.UtcNow.AddMinutes(-5).ToString("O");

        var newAuth = new AuthResponseDto
        {
            AccessToken = "new-access-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserInfoDto { Id = "u1", Email = "a@b.com", FullName = "User" }
        };
        var accountSvc = BuildAccountService(newAuth, "new-refresh-token");

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(Mock.Of<IAuthenticationService>(a =>
            a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties?>()) == Task.CompletedTask));
        services.AddSingleton(accountSvc);
        var sp = services.BuildServiceProvider();

        var claims = new List<Claim>
        {
            new("token_expires_at", expiredAt),
            new("refresh_token", "old-refresh-token"),
            new("access_token", "old-access-token")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity),
            RequestServices = sp
        };
        httpContext.Request.Path = "/products";

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = "Products",
            ActionName = "Index"
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), actionDescriptor);
        var ctx = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        var delegateCalled = false;
        ActionExecutionDelegate next = () =>
        {
            delegateCalled = true;
            return BuildDelegate()();
        };

        await filter.OnActionExecutionAsync(ctx, next);

        delegateCalled.Should().BeTrue();
        ctx.Result.Should().BeNull();
    }

    // ── Invalid date format ───────────────────────────────────────────────────

    [Fact]
    public async Task OnActionExecution_InvalidExpiryDateFormat_PassesThrough()
    {
        var filter = new TokenExpiryFilter();
        var (ctx, _) = BuildContext(authenticated: true, expiresAt: "not-a-valid-date");
        var delegateCalled = false;
        ActionExecutionDelegate next = () =>
        {
            delegateCalled = true;
            return BuildDelegate()();
        };

        await filter.OnActionExecutionAsync(ctx, next);

        delegateCalled.Should().BeTrue();
        ctx.Result.Should().BeNull();
    }

    // ── Expired token — refresh throws (API down) ────────────────────────────

    [Fact]
    public async Task OnActionExecution_ExpiredToken_RefreshApiThrows_SignsOutAndRedirects()
    {
        var filter = new TokenExpiryFilter();
        var expiredAt = DateTime.UtcNow.AddMinutes(-5).ToString("O");

        var accountSvcMock = new Mock<IAccountService>();
        accountSvcMock.Setup(s => s.RefreshAsync(It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("API down"));

        var authSvcMock = new Mock<IAuthenticationService>();
        authSvcMock
            .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(authSvcMock.Object);
        services.AddSingleton(accountSvcMock.Object);
        var sp = services.BuildServiceProvider();

        var claims = new List<Claim>
        {
            new("token_expires_at", expiredAt),
            new("refresh_token", "old-refresh")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity),
            RequestServices = sp
        };
        httpContext.Request.Path = "/products";

        var descriptor = new ControllerActionDescriptor { ControllerName = "Products", ActionName = "Index" };
        var actionContext = new ActionContext(httpContext, new RouteData(), descriptor);
        var ctx = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        await filter.OnActionExecutionAsync(ctx, () => BuildDelegate()());

        ctx.Result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Contain("/account/login");
    }

    // ── Refresh succeeds but newExpiry is null (non-JWT token) ───────────────

    [Fact]
    public async Task OnActionExecution_ExpiredToken_RefreshSucceeds_NullExpiry_ContinuesWithoutExpiryClaim()
    {
        // A fake access token with 3 parts but no "exp" in payload
        const string noExpToken = "eyJhbGciOiJub25lIn0.eyJzdWIiOiJ1MSJ9.sig";
        var filter = new TokenExpiryFilter();
        var expiredAt = DateTime.UtcNow.AddMinutes(-5).ToString("O");

        var newAuth = new AuthResponseDto
        {
            AccessToken = noExpToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserInfoDto { Id = "u1", Email = "a@b.com", FullName = "User" }
        };
        var accountSvc = BuildAccountService(newAuth, newRefreshToken: null);

        var services = new ServiceCollection();
        services.AddSingleton<IAuthenticationService>(Mock.Of<IAuthenticationService>(a =>
            a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties?>()) == Task.CompletedTask));
        services.AddSingleton(accountSvc);
        var sp = services.BuildServiceProvider();

        var claims = new List<Claim>
        {
            new("token_expires_at", expiredAt),
            new("refresh_token", "old-refresh-token"),
            new("access_token", "old-access-token")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity),
            RequestServices = sp
        };
        httpContext.Request.Path = "/products";

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = "Products",
            ActionName = "Index"
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), actionDescriptor);
        var ctx = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        var delegateCalled = false;
        ActionExecutionDelegate next = () =>
        {
            delegateCalled = true;
            return BuildDelegate()();
        };

        await filter.OnActionExecutionAsync(ctx, next);

        delegateCalled.Should().BeTrue();
        ctx.Result.Should().BeNull();
    }

    // ── Expired token — refresh fails ─────────────────────────────────────────

    [Fact]
    public async Task OnActionExecution_ExpiredToken_NoRefreshToken_SignsOutAndRedirects()
    {
        var filter = new TokenExpiryFilter();
        var expiredAt = DateTime.UtcNow.AddMinutes(-5).ToString("O");
        var (ctx, authSvcMock) = BuildContext(
            authenticated: true,
            expiresAt: expiredAt,
            refreshToken: null);

        await filter.OnActionExecutionAsync(ctx, () => BuildDelegate()());

        ctx.Result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Contain("/account/login");
        authSvcMock.Verify(a =>
            a.SignOutAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<AuthenticationProperties?>()),
            Times.Once);
    }

    [Fact]
    public async Task OnActionExecution_ExpiredToken_RefreshReturnsNull_SignsOutAndRedirects()
    {
        var filter = new TokenExpiryFilter();
        var expiredAt = DateTime.UtcNow.AddMinutes(-5).ToString("O");

        var accountSvcMock = new Mock<IAccountService>();
        accountSvcMock.Setup(s => s.RefreshAsync(It.IsAny<string>()))
            .ReturnsAsync(((AuthResponseDto?)null, (string?)null));

        var authSvcMock = new Mock<IAuthenticationService>();
        authSvcMock
            .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(authSvcMock.Object);
        services.AddSingleton(accountSvcMock.Object);
        var sp = services.BuildServiceProvider();

        var claims = new List<Claim>
        {
            new("token_expires_at", expiredAt),
            new("refresh_token", "old-refresh")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity),
            RequestServices = sp
        };
        httpContext.Request.Path = "/products";

        var descriptor = new ControllerActionDescriptor { ControllerName = "Products", ActionName = "Index" };
        var actionContext = new ActionContext(httpContext, new RouteData(), descriptor);
        var ctx = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), new object());

        await filter.OnActionExecutionAsync(ctx, () => BuildDelegate()());

        ctx.Result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Contain("/account/login");
    }
}
