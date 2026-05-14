using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Infrastructure.Identity;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;

namespace RetailsEcosystem.Customer.Tests.Infrastructure;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IConfiguration _config;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:RefreshTokenExpiryDays"] = "7",
                ["JwtSettings:AccessTokenExpiryMinutes"] = "15"
            })
            .Build();

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _tokenServiceMock
            .Setup(t => t.GenerateAccessToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns("access-token-value");

        _tokenServiceMock
            .Setup(t => t.GenerateRefreshToken())
            .Returns("refresh-token-value");

        _sut = new AuthService(
            _userManagerMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenRepoMock.Object,
            _config,
            _unitOfWorkMock.Object);
    }

    private static ApplicationUser ActiveUser(string id = "user-1", string email = "user@test.com") => new()
    {
        Id = id,
        Email = email,
        FullName = "Test User",
        UserName = email,
        IsActive = true
    };

    // ── RegisterAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Register_NewEmail_CreatesUserWithCustomerRole()
    {
        var user = ActiveUser();
        _userManagerMock.Setup(m => m.FindByEmailAsync("user@test.com"))
            .ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Customer"]);
        _refreshTokenRepoMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var dto = new RegisterDto { Email = "user@test.com", Password = "Pass1!", FullName = "Test User", ConfirmPassword = "Pass1!" };
        var (response, refreshToken) = await _sut.RegisterAsync(dto);

        response.AccessToken.Should().Be("access-token-value");
        refreshToken.Should().Be("refresh-token-value");
        _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"), Times.Once);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync("user@test.com"))
            .ReturnsAsync(ActiveUser());

        var dto = new RegisterDto { Email = "user@test.com", Password = "Pass1!", FullName = "Test", ConfirmPassword = "Pass1!" };
        var act = () => _sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Register_CreateFails_ThrowsInvalidOperationException()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak." }));

        var dto = new RegisterDto { Email = "x@x.com", Password = "weak", FullName = "X", ConfirmPassword = "weak" };
        var act = () => _sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Password too weak*");
    }

    // ── LoginAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokenPair()
    {
        var user = ActiveUser();
        _userManagerMock.Setup(m => m.FindByEmailAsync("user@test.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "Pass1!")).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(["Customer"]);
        _refreshTokenRepoMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        var dto = new LoginDto { Email = "user@test.com", Password = "Pass1!" };
        var (response, refreshToken) = await _sut.LoginAsync(dto);

        response.AccessToken.Should().Be("access-token-value");
        refreshToken.Should().Be("refresh-token-value");
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var dto = new LoginDto { Email = "nobody@test.com", Password = "any" };
        var act = () => _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*email or password*");
    }

    [Fact]
    public async Task Login_InactiveUser_ThrowsUnauthorizedAccessException()
    {
        var user = ActiveUser();
        user.IsActive = false;
        _userManagerMock.Setup(m => m.FindByEmailAsync("user@test.com")).ReturnsAsync(user);

        var dto = new LoginDto { Email = "user@test.com", Password = "Pass1!" };
        var act = () => _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*disabled*");
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = ActiveUser();
        _userManagerMock.Setup(m => m.FindByEmailAsync("user@test.com")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        var dto = new LoginDto { Email = "user@test.com", Password = "wrong" };
        var act = () => _sut.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*email or password*");
    }

    // ── RefreshTokenAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task Refresh_ValidActiveToken_RotatesAndReturnsNewPair()
    {
        var user = ActiveUser();
        var stored = new RefreshToken
        {
            Token = "old-token",
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("old-token")).ReturnsAsync(stored);
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(["Customer"]);
        _refreshTokenRepoMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        var (response, newRefreshToken) = await _sut.RefreshTokenAsync("old-token");

        response.AccessToken.Should().Be("access-token-value");
        newRefreshToken.Should().Be("refresh-token-value");
        stored.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task Refresh_TokenNotFound_ThrowsUnauthorizedAccessException()
    {
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken?)null);

        var act = () => _sut.RefreshTokenAsync("missing-token");

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Refresh_RevokedToken_ThrowsUnauthorizedAccessException()
    {
        var stored = new RefreshToken
        {
            Token = "revoked",
            UserId = "user-1",
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = true
        };
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("revoked")).ReturnsAsync(stored);

        var act = () => _sut.RefreshTokenAsync("revoked");

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*expired or been revoked*");
    }

    [Fact]
    public async Task Refresh_ExpiredToken_ThrowsUnauthorizedAccessException()
    {
        var stored = new RefreshToken
        {
            Token = "expired",
            UserId = "user-1",
            ExpiryDate = DateTime.UtcNow.AddDays(-1),
            IsRevoked = false
        };
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("expired")).ReturnsAsync(stored);

        var act = () => _sut.RefreshTokenAsync("expired");

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*expired or been revoked*");
    }

    [Fact]
    public async Task Refresh_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        var stored = new RefreshToken
        {
            Token = "good-token",
            UserId = "ghost-user",
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("good-token")).ReturnsAsync(stored);
        _userManagerMock.Setup(m => m.FindByIdAsync("ghost-user"))
            .ReturnsAsync((ApplicationUser?)null);

        var act = () => _sut.RefreshTokenAsync("good-token");

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*User not found*");
    }

    [Fact]
    public async Task Refresh_InactiveUser_ThrowsUnauthorizedAccessException()
    {
        var user = ActiveUser();
        user.IsActive = false;
        var stored = new RefreshToken
        {
            Token = "tok",
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("tok")).ReturnsAsync(stored);
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);

        var act = () => _sut.RefreshTokenAsync("tok");

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*disabled*");
    }

    // ── LogoutAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Logout_ExistingToken_RevokesAllForUser()
    {
        var stored = new RefreshToken
        {
            Token = "valid-refresh",
            UserId = "user-1",
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("valid-refresh")).ReturnsAsync(stored);
        _refreshTokenRepoMock.Setup(r => r.RevokeAllForUserAsync("user-1")).Returns(Task.CompletedTask);

        await _sut.LogoutAsync("valid-refresh");

        _refreshTokenRepoMock.Verify(r => r.RevokeAllForUserAsync("user-1"), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Logout_TokenNotFound_DoesNotThrow()
    {
        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken?)null);

        var act = () => _sut.LogoutAsync("missing");

        await act.Should().NotThrowAsync();
        _refreshTokenRepoMock.Verify(r => r.RevokeAllForUserAsync(It.IsAny<string>()), Times.Never);
    }

    // ── GetMeAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMe_ExistingUser_ReturnsMeDto()
    {
        var user = ActiveUser("user-1", "me@test.com");
        _userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(["Customer"]);

        var result = await _sut.GetMeAsync("user-1");

        result.Id.Should().Be("user-1");
        result.Email.Should().Be("me@test.com");
        result.Roles.Should().Contain("Customer");
    }

    [Fact]
    public async Task GetMe_UserNotFound_ThrowsInvalidOperationException()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("ghost")).ReturnsAsync((ApplicationUser?)null);

        var act = () => _sut.GetMeAsync("ghost");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User not found*");
    }
}
