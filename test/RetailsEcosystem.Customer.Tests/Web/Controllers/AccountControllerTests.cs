using System.Net;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using RetailsEcosystem.Customer.Web.Controllers;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Models.Account;

namespace RetailsEcosystem.Customer.Tests.Web.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _accountSvcMock = new();

    private static AuthResponseDto ValidAuth(string token = "tok") => new()
    {
        AccessToken = token,
        ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        User = new UserInfoDto { Id = "u1", Email = "a@b.com", FullName = "User", Roles = ["Customer"] }
    };

    private static CustomerDto ValidProfile() => new()
    {
        Id = "u1",
        Email = "a@b.com",
        FullName = "User",
        AvatarUrl = null
    };

    private AccountController BuildController(
        bool authenticated = false,
        string? accessToken = null,
        bool localUrl = true)
    {
        var authSvcMock = new Mock<IAuthenticationService>();
        authSvcMock
            .Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(), It.IsAny<string?>(),
                It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);
        authSvcMock
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<AuthenticationProperties?>()))
            .Returns(Task.CompletedTask);

        var tempDataMock = new Mock<ITempDataDictionary>();
        var tempDataFactory = new Mock<ITempDataDictionaryFactory>();
        tempDataFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>()))
            .Returns(tempDataMock.Object);

        var services = new ServiceCollection();
        services.AddSingleton(authSvcMock.Object);
        services.AddSingleton(tempDataFactory.Object);
        var sp = services.BuildServiceProvider();

        var claims = new List<Claim>();
        if (accessToken != null) claims.Add(new Claim("access_token", accessToken));
        if (authenticated)
        {
            claims.Add(new Claim(ClaimTypes.Name, "User"));
            claims.Add(new Claim(ClaimTypes.Email, "a@b.com"));
            claims.Add(new Claim(ClaimTypes.Role, "Customer"));
            claims.Add(new Claim("refresh_token", "refresh-tok"));
        }

        var identity = authenticated
            ? new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
            : new ClaimsIdentity(claims);

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity),
            RequestServices = sp
        };

        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(localUrl);

        var controller = new AccountController(_accountSvcMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        controller.Url = urlHelperMock.Object;
        return controller;
    }

    // ── GET /account/login ────────────────────────────────────────────────────

    [Fact]
    public void Login_Get_Unauthenticated_ReturnsView()
    {
        var sut = BuildController(authenticated: false);

        var result = sut.Login((string?)null);

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Login_Get_Authenticated_RedirectsToHome()
    {
        var sut = BuildController(authenticated: true);

        var result = sut.Login((string?)null);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
    }

    // ── POST /account/login ───────────────────────────────────────────────────

    [Fact]
    public async Task Login_Post_InvalidModel_ReturnsView()
    {
        var sut = BuildController();
        sut.ModelState.AddModelError("Email", "Required");
        var model = new LoginViewModel();

        var result = await sut.Login(model);

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Login_Post_ValidCredentials_SignsInAndRedirectsToHome()
    {
        _accountSvcMock.Setup(s => s.LoginAsync("a@b.com", "Pass1!"))
            .ReturnsAsync((ValidAuth(), "refresh-tok"));
        _accountSvcMock.Setup(s => s.GetProfileAsync("tok"))
            .ReturnsAsync(ValidProfile());
        var sut = BuildController(localUrl: false);
        var model = new LoginViewModel { Email = "a@b.com", Password = "Pass1!" };

        var result = await sut.Login(model);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Login_Post_ValidReturnUrl_RedirectsToReturnUrl()
    {
        _accountSvcMock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((ValidAuth(), "rt"));
        _accountSvcMock.Setup(s => s.GetProfileAsync(It.IsAny<string>()))
            .ReturnsAsync(ValidProfile());
        var sut = BuildController(localUrl: true);
        var model = new LoginViewModel { Email = "a@b.com", Password = "Pass1!", ReturnUrl = "/cart" };

        var result = await sut.Login(model);

        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Be("/cart");
    }

    [Fact]
    public async Task Login_Post_UnauthorizedHttpException_ReturnsViewWithError()
    {
        _accountSvcMock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("Invalid email or password.", null, HttpStatusCode.Unauthorized));
        var sut = BuildController();
        var model = new LoginViewModel { Email = "x@x.com", Password = "wrong" };

        var result = await sut.Login(model);

        result.Should().BeOfType<ViewResult>();
        sut.ModelState.IsValid.Should().BeFalse();
    }

    // ── GET /account/register ─────────────────────────────────────────────────

    [Fact]
    public void Register_Get_Unauthenticated_ReturnsView()
    {
        var sut = BuildController(authenticated: false);

        var result = sut.Register();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Register_Get_Authenticated_RedirectsToHome()
    {
        var sut = BuildController(authenticated: true);

        var result = sut.Register();

        result.Should().BeOfType<RedirectToActionResult>();
    }

    // ── POST /account/register ────────────────────────────────────────────────

    [Fact]
    public async Task Register_Post_InvalidModel_ReturnsView()
    {
        var sut = BuildController();
        sut.ModelState.AddModelError("Email", "Required");

        var result = await sut.Register(new RegisterViewModel());

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Register_Post_ValidPayload_SignsInAndRedirects()
    {
        _accountSvcMock.Setup(s => s.RegisterAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((ValidAuth(), "rt"));
        var sut = BuildController();
        var model = new RegisterViewModel
        {
            FullName = "User",
            Email = "a@b.com",
            Password = "Pass1!",
            ConfirmPassword = "Pass1!"
        };

        var result = await sut.Register(model);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task Register_Post_BadRequestException_ReturnsViewWithError()
    {
        _accountSvcMock.Setup(s => s.RegisterAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("Bad", null, HttpStatusCode.BadRequest));
        var sut = BuildController();
        var model = new RegisterViewModel
        {
            FullName = "User", Email = "a@b.com", Password = "Pass1!", ConfirmPassword = "Pass1!"
        };

        var result = await sut.Register(model);

        result.Should().BeOfType<ViewResult>();
        sut.ModelState.IsValid.Should().BeFalse();
    }

    // ── GET /account/change-password ──────────────────────────────────────────

    [Fact]
    public void ChangePassword_Get_ReturnsView()
    {
        var sut = BuildController(authenticated: true, accessToken: "tok");

        var result = sut.ChangePassword();

        result.Should().BeOfType<ViewResult>();
    }

    // ── POST /account/change-password ─────────────────────────────────────────

    [Fact]
    public async Task ChangePassword_Post_InvalidModel_ReturnsView()
    {
        var sut = BuildController(authenticated: true, accessToken: "tok");
        sut.ModelState.AddModelError("CurrentPassword", "Required");

        var result = await sut.ChangePassword(new ChangePasswordViewModel());

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task ChangePassword_Post_ValidPayload_RedirectsToProfile()
    {
        _accountSvcMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        var sut = BuildController(authenticated: true, accessToken: "tok");
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "old",
            NewPassword = "New1!",
            ConfirmNewPassword = "New1!"
        };

        var result = await sut.ChangePassword(model);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Profile");
    }

    [Fact]
    public async Task ChangePassword_Post_HttpException_ReturnsViewWithError()
    {
        _accountSvcMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("Current password is incorrect."));
        var sut = BuildController(authenticated: true, accessToken: "tok");
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "wrong",
            NewPassword = "New1!",
            ConfirmNewPassword = "New1!"
        };

        var result = await sut.ChangePassword(model);

        result.Should().BeOfType<ViewResult>();
        sut.ModelState.IsValid.Should().BeFalse();
    }

    // ── POST /account/logout ──────────────────────────────────────────────────

    [Fact]
    public async Task Logout_CallsLogoutServiceAndRedirects()
    {
        _accountSvcMock.Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        var sut = BuildController(authenticated: true, accessToken: "tok");

        var result = await sut.Logout();

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
        _accountSvcMock.Verify(s => s.LogoutAsync("tok"), Times.Once);
    }

    [Fact]
    public async Task Logout_NoAccessToken_StillRedirects()
    {
        var sut = BuildController(authenticated: false);

        var result = await sut.Logout();

        result.Should().BeOfType<RedirectToActionResult>();
        _accountSvcMock.Verify(s => s.LogoutAsync(It.IsAny<string>()), Times.Never);
    }

    // ── GET /account/profile ──────────────────────────────────────────────────

    [Fact]
    public async Task Profile_Get_AuthenticatedUser_ReturnsViewWithProfile()
    {
        _accountSvcMock.Setup(s => s.GetProfileAsync("tok"))
            .ReturnsAsync(new CustomerDto
            {
                Id       = "u1",
                FullName = "User",
                Email    = "a@b.com",
                AvatarUrl = null
            });
        var sut = BuildController(authenticated: true, accessToken: "tok");

        var result = await sut.Profile();

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<ProfileViewModel>();
    }

    // ── POST /account/profile ─────────────────────────────────────────────────

    [Fact]
    public async Task Profile_Post_InvalidModel_ReturnsView()
    {
        var sut = BuildController(authenticated: true, accessToken: "tok");
        sut.ModelState.AddModelError("FullName", "Required");

        var result = await sut.Profile(new ProfileViewModel());

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Profile_Post_ValidPayload_RedirectsToProfile()
    {
        _accountSvcMock.Setup(s => s.UpdateProfileAsync("tok", It.IsAny<UpdateProfileDto>()))
            .Returns(Task.CompletedTask);
        var sut   = BuildController(authenticated: true, accessToken: "tok");
        var model = new ProfileViewModel { FullName = "Updated User" };

        var result = await sut.Profile(model);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Profile");
    }

    [Fact]
    public async Task Profile_Post_HttpException_ReturnsViewWithError()
    {
        _accountSvcMock.Setup(s => s.UpdateProfileAsync("tok", It.IsAny<UpdateProfileDto>()))
            .ThrowsAsync(new HttpRequestException("Update failed"));
        var sut   = BuildController(authenticated: true, accessToken: "tok");
        var model = new ProfileViewModel { FullName = "Updated User" };

        var result = await sut.Profile(model);

        result.Should().BeOfType<ViewResult>();
        sut.ModelState.IsValid.Should().BeFalse();
    }

    // ── POST /account/profile/avatar ──────────────────────────────────────────

    [Fact]
    public async Task UploadAvatar_NullFile_ReturnsBadRequest()
    {
        var sut = BuildController(authenticated: true, accessToken: "tok");

        var result = await sut.UploadAvatar(null!);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UploadAvatar_ValidFile_ReturnsOkWithUrl()
    {
        _accountSvcMock.Setup(s => s.UploadAvatarAsync("tok", It.IsAny<IFormFile>()))
            .ReturnsAsync("https://cdn.example.com/avatar.jpg");
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        var sut = BuildController(authenticated: true, accessToken: "tok");

        var result = await sut.UploadAvatar(fileMock.Object);

        var ok   = result.Should().BeOfType<OkObjectResult>().Subject;
        var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
        json.Should().Contain("url");
    }

    [Fact]
    public async Task UploadAvatar_UploadThrows_Returns500()
    {
        _accountSvcMock.Setup(s => s.UploadAvatarAsync("tok", It.IsAny<IFormFile>()))
            .ThrowsAsync(new Exception("Storage error"));
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(1024);
        var sut = BuildController(authenticated: true, accessToken: "tok");

        var result = await sut.UploadAvatar(fileMock.Object);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }
}
