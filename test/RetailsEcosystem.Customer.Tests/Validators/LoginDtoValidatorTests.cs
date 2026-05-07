using FluentAssertions;
using RetailsEcosystem.Customer.Application.Validators;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;

namespace RetailsEcosystem.Customer.Tests.Validators;

public class LoginDtoValidatorTests
{
    private readonly LoginDtoValidator _sut = new();

    [Fact]
    public void Validate_EmptyEmail_ReturnsError()
    {
        var result = _sut.Validate(new LoginDto { Email = "", Password = "ValidPass1!" });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_InvalidEmailFormat_ReturnsError()
    {
        var result = _sut.Validate(new LoginDto { Email = "not-an-email", Password = "ValidPass1!" });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_EmptyPassword_ReturnsError()
    {
        var result = _sut.Validate(new LoginDto { Email = "user@example.com", Password = "" });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_ValidDto_PassesWithNoErrors()
    {
        var result = _sut.Validate(new LoginDto { Email = "user@example.com", Password = "AnyPass1!" });
        result.IsValid.Should().BeTrue();
    }
}
