using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;

namespace RetailsEcosystem.Customer.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            Mock.Of<ILookupNormalizer>(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<ApplicationUser>>>()
        );
        _sut = new CustomerService(_userManagerMock.Object);
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_UserExists_ReturnsCustomerDto()
    {
        var user = new CustomerBuilder().WithId("user-123").WithEmail("test@example.com").Build();
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(["Customer"]);

        var result = await _sut.GetByIdAsync("user-123");

        result.Id.Should().Be("user-123");
        result.Email.Should().Be("test@example.com");
        result.Roles.Should().Contain("Customer");
    }

    [Fact]
    public async Task GetByIdAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

        var act = () => _sut.GetByIdAsync("unknown");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── UpdateProfileAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateProfileAsync_ValidUser_UpdatesSuccessfully()
    {
        var user = new CustomerBuilder().WithId("user-123").Build();
        var dto = new UpdateProfileDto { FullName = "Updated Name", Address = "123 Main St" };
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        await _sut.UpdateProfileAsync("user-123", dto);

        user.FullName.Should().Be("Updated Name");
        user.Address.Should().Be("123 Main St");
    }

    [Fact]
    public async Task UpdateProfileAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

        var act = () => _sut.UpdateProfileAsync("unknown", new UpdateProfileDto { FullName = "Test" });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateProfileAsync_UpdateFails_ThrowsInvalidOperationException()
    {
        var user = new CustomerBuilder().WithId("user-123").Build();
        var failResult = IdentityResult.Failed(new IdentityError { Description = "Update failed" });
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(failResult);

        var act = () => _sut.UpdateProfileAsync("user-123", new UpdateProfileDto { FullName = "Test" });

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Update failed*");
    }

    // ── ChangePasswordAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ChangePasswordAsync_CorrectPassword_ChangesSuccessfully()
    {
        var user = new CustomerBuilder().WithId("user-123").Build();
        var dto = new ChangePasswordDto { CurrentPassword = "OldPass1!", NewPassword = "NewPass1!" };
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, "OldPass1!", "NewPass1!"))
            .ReturnsAsync(IdentityResult.Success);

        var act = () => _sut.ChangePasswordAsync("user-123", dto);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ChangePasswordAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

        var act = () => _sut.ChangePasswordAsync("unknown", new ChangePasswordDto { CurrentPassword = "Old", NewPassword = "New" });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ChangePasswordAsync_WrongPassword_ThrowsInvalidOperationException()
    {
        var user = new CustomerBuilder().WithId("user-123").Build();
        var failResult = IdentityResult.Failed(new IdentityError { Description = "Incorrect password." });
        var dto = new ChangePasswordDto { CurrentPassword = "WrongPass!", NewPassword = "NewPass1!" };
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, "WrongPass!", "NewPass1!"))
            .ReturnsAsync(failResult);

        var act = () => _sut.ChangePasswordAsync("user-123", dto);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Incorrect password*");
    }

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_NoSearch_ReturnsAllUsers()
    {
        var users = new List<ApplicationUser>
        {
            new CustomerBuilder().WithId("a").WithFullName("Alice Smith").Build(),
            new CustomerBuilder().WithId("b").WithFullName("Bob Jones").Build()
        };
        _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Customer"]);

        var result = await _sut.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 }, null);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_WithSearch_FiltersByFullName()
    {
        var users = new List<ApplicationUser>
        {
            new CustomerBuilder().WithId("a").WithFullName("Alice Smith").WithEmail("alice@test.com").Build(),
            new CustomerBuilder().WithId("b").WithFullName("Bob Jones").WithEmail("bob@test.com").Build()
        };
        _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Customer"]);

        var result = await _sut.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 10 }, "Alice");

        result.Items.Should().HaveCount(1);
        result.Items.First().FullName.Should().Be("Alice Smith");
    }

    [Fact]
    public async Task GetAllAsync_Paginated_ReturnsCorrectPage()
    {
        var users = Enumerable.Range(1, 15)
            .Select(i => new CustomerBuilder().WithId($"user-{i}").WithFullName($"User {i:D2}").Build())
            .ToList();
        _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
        _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(["Customer"]);

        var result = await _sut.GetAllAsync(new PagedRequest { PageNumber = 2, PageSize = 10 }, null);

        result.Items.Should().HaveCount(5);
        result.TotalPage.Should().Be(2);
    }

    // ── UpdateStatusAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatusAsync_Deactivate_SetsIsActiveToFalse()
    {
        var user = new CustomerBuilder().WithId("user-123").WithIsActive(true).Build();
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        await _sut.UpdateStatusAsync("user-123", false);

        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateStatusAsync_Activate_SetsIsActiveToTrue()
    {
        var user = new CustomerBuilder().WithId("user-123").WithIsActive(false).Build();
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        await _sut.UpdateStatusAsync("user-123", true);

        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStatusAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("unknown")).ReturnsAsync((ApplicationUser?)null);

        var act = () => _sut.UpdateStatusAsync("unknown", true);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateStatusAsync_UpdateFails_ThrowsInvalidOperationException()
    {
        var user = new CustomerBuilder().WithId("user-123").Build();
        var failResult = IdentityResult.Failed(new IdentityError { Description = "Update failed" });
        _userManagerMock.Setup(m => m.FindByIdAsync("user-123")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(failResult);

        var act = () => _sut.UpdateStatusAsync("user-123", false);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
