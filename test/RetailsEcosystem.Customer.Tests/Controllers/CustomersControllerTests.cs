using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.API.Services;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerService>    _customerServiceMock = new();
    private readonly Mock<IFileStorageService> _fileStorageMock     = new();
    private readonly CustomersController _sut;

    public CustomersControllerTests()
    {
        _sut = new CustomersController(_customerServiceMock.Object, _fileStorageMock.Object);
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

    // ── GetMe ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetMe_WhenCalled_ReturnsOkWithCustomerDto()
    {
        var customerDto = new CustomerDto { Id = "user-123", Email = "test@example.com", FullName = "Test User" };
        _customerServiceMock.Setup(s => s.GetByIdAsync("user-123")).ReturnsAsync(customerDto);

        var result = await _sut.GetMe();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(customerDto);
    }

    [Fact]
    public async Task GetMe_UserNotFound_ThrowsKeyNotFoundException()
    {
        _customerServiceMock.Setup(s => s.GetByIdAsync("user-123"))
            .ThrowsAsync(new KeyNotFoundException("User not found."));

        var act = () => _sut.GetMe();

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── UpdateMe ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateMe_ValidDto_ReturnsNoContent()
    {
        var dto = new UpdateProfileDto { FullName = "Updated Name" };
        _customerServiceMock.Setup(s => s.UpdateProfileAsync("user-123", dto)).Returns(Task.CompletedTask);

        var result = await _sut.UpdateMe(dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateMe_UserNotFound_ThrowsKeyNotFoundException()
    {
        _customerServiceMock.Setup(s => s.UpdateProfileAsync("user-123", It.IsAny<UpdateProfileDto>()))
            .ThrowsAsync(new KeyNotFoundException("User not found."));

        var act = () => _sut.UpdateMe(new UpdateProfileDto { FullName = "Test" });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── ChangePassword ────────────────────────────────────────────────────────

    [Fact]
    public async Task ChangePassword_ValidDto_ReturnsNoContent()
    {
        var dto = new ChangePasswordDto { CurrentPassword = "OldPass1!", NewPassword = "NewPass1!" };
        _customerServiceMock.Setup(s => s.ChangePasswordAsync("user-123", dto)).Returns(Task.CompletedTask);

        var result = await _sut.ChangePassword(dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangePassword_WrongPassword_ThrowsInvalidOperationException()
    {
        _customerServiceMock.Setup(s => s.ChangePasswordAsync("user-123", It.IsAny<ChangePasswordDto>()))
            .ThrowsAsync(new InvalidOperationException("Incorrect password."));

        var act = () => _sut.ChangePassword(new ChangePasswordDto { CurrentPassword = "Wrong!", NewPassword = "New!" });

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithPagedResult()
    {
        var pagedResult = new PagedResult<CustomerDto>([], new PagedRequest { PageNumber = 1, PageSize = 10 }, 0);
        _customerServiceMock.Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>(), null)).ReturnsAsync(pagedResult);

        var result = await _sut.GetAll();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(pagedResult);
    }

    [Fact]
    public async Task GetAll_WithSearch_PassesSearchToService()
    {
        var pagedResult = new PagedResult<CustomerDto>([], new PagedRequest { PageNumber = 1, PageSize = 10 }, 0);
        _customerServiceMock.Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>(), "Alice")).ReturnsAsync(pagedResult);

        var result = await _sut.GetAll(search: "Alice");

        result.Result.Should().BeOfType<OkObjectResult>();
        _customerServiceMock.Verify(s => s.GetAllAsync(It.IsAny<PagedRequest>(), "Alice"), Times.Once);
    }

    // ── UpdateStatus ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatus_ValidRequest_ReturnsNoContent()
    {
        _customerServiceMock.Setup(s => s.UpdateStatusAsync("user-456", false)).Returns(Task.CompletedTask);

        var result = await _sut.UpdateStatus("user-456", new UpdateCustomerStatusDto { IsActive = false });

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateStatus_UserNotFound_ThrowsKeyNotFoundException()
    {
        _customerServiceMock.Setup(s => s.UpdateStatusAsync("unknown", It.IsAny<bool>()))
            .ThrowsAsync(new KeyNotFoundException("User not found."));

        var act = () => _sut.UpdateStatus("unknown", new UpdateCustomerStatusDto { IsActive = false });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── UploadAvatar ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UploadAvatar_WithFile_CallsStorageAndReturnsOkWithUrl()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        _fileStorageMock.Setup(s => s.SaveFilesAsync(It.IsAny<List<IFormFile>>()))
            .ReturnsAsync(["https://cdn.example.com/avatars/user-123.jpg"]);
        _customerServiceMock.Setup(s => s.UpdateAvatarAsync("user-123", "https://cdn.example.com/avatars/user-123.jpg"))
            .Returns(Task.CompletedTask);

        var result = await _sut.UploadAvatar(fileMock.Object);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UploadAvatar_NullFile_ReturnsBadRequest()
    {
        var result = await _sut.UploadAvatar(null!);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UploadAvatar_EmptyFile_ReturnsBadRequest()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        var result = await _sut.UploadAvatar(fileMock.Object);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
