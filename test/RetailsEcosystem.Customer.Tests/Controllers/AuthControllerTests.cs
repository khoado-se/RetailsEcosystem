using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly Mock<IValidator<RegisterDto>> _registerValidatorMock = new();
    private readonly Mock<IValidator<LoginDto>> _loginValidatorMock = new();
    private readonly AuthController _sut;

    private static readonly AuthResponseDto ValidAuthResponse = new()
    {
        AccessToken = "token-abc",
        ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        User = new UserInfoDto { Id = "user-123", Email = "test@example.com", FullName = "Test User" }
    };

    public AuthControllerTests()
    {
        _sut = new AuthController(_authServiceMock.Object, _registerValidatorMock.Object, _loginValidatorMock.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "user-123")
                ], "TestAuth"))
            }
        };
    }

    // ── Register ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Register_ValidDto_ReturnsOkWithAuthResponse()
    {
        var dto = new RegisterDto { FullName = "Test User", Email = "test@example.com", Password = "Pass1!", ConfirmPassword = "Pass1!" };
        _registerValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync((ValidAuthResponse, "refresh-token"));

        var result = await _sut.Register(dto);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(ValidAuthResponse);
    }

    [Fact]
    public async Task Register_InvalidDto_ReturnsBadRequest()
    {
        var dto = new RegisterDto { Email = "" };
        var failures = new[] { new ValidationFailure("Email", "Email is required.") };
        _registerValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _sut.Register(dto);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithAuthResponse()
    {
        var dto = new LoginDto { Email = "test@example.com", Password = "Pass1!" };
        _loginValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync((ValidAuthResponse, "refresh-token"));

        var result = await _sut.Login(dto);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(ValidAuthResponse);
    }

    [Fact]
    public async Task Login_InvalidDto_ReturnsBadRequest()
    {
        var dto = new LoginDto { Email = "" };
        var failures = new[] { new ValidationFailure("Email", "Email is required.") };
        _loginValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _sut.Login(dto);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    // ── Refresh ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_ValidCookie_ReturnsOkWithAuthResponse()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("Cookie", "refreshToken=valid-token");
        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
        _authServiceMock.Setup(s => s.RefreshTokenAsync("valid-token"))
            .ReturnsAsync((ValidAuthResponse, "new-refresh-token"));

        var result = await _sut.Refresh();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(ValidAuthResponse);
    }

    [Fact]
    public async Task Refresh_MissingCookie_ReturnsUnauthorized()
    {
        _sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await _sut.Refresh();

        result.Should().BeOfType<UnauthorizedObjectResult>()
            .Which.StatusCode.Should().Be(401);
    }

    // ── Logout ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Logout_WithRefreshCookie_RevokesAndReturnsNoContent()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("Cookie", "refreshToken=valid-token");
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "user-123")
        ], "TestAuth"));
        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
        _authServiceMock.Setup(s => s.LogoutAsync("valid-token")).Returns(Task.CompletedTask);

        var result = await _sut.Logout();

        result.Should().BeOfType<NoContentResult>();
        _authServiceMock.Verify(s => s.LogoutAsync("valid-token"), Times.Once);
    }

    [Fact]
    public async Task Logout_WithoutRefreshCookie_ReturnsNoContentWithoutCallingService()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "user-123")
        ], "TestAuth"));
        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var result = await _sut.Logout();

        result.Should().BeOfType<NoContentResult>();
        _authServiceMock.Verify(s => s.LogoutAsync(It.IsAny<string>()), Times.Never);
    }

    // ── Me ────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Me_AuthenticatedUser_ReturnsOkWithMeResponse()
    {
        var meResponse = new MeResponseDto { Id = "user-123", Email = "test@example.com", FullName = "Test User" };
        _authServiceMock.Setup(s => s.GetMeAsync("user-123")).ReturnsAsync(meResponse);

        var result = await _sut.Me();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(meResponse);
    }

    [Fact]
    public async Task Me_MissingNameIdentifierClaim_ReturnsUnauthorized()
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity("TestAuth"))
            }
        };

        var result = await _sut.Me();

        result.Should().BeOfType<UnauthorizedResult>();
    }
}
