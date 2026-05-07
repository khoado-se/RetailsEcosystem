using FluentAssertions;
using RetailsEcosystem.Customer.Application.Validators;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;

namespace RetailsEcosystem.Customer.Tests.Validators;

public class RegisterDtoValidatorTests
{
    private readonly RegisterDtoValidator _sut = new();

    private static RegisterDto ValidDto() => new()
    {
        FullName = "Alice Smith",
        Email = "alice@example.com",
        Password = "Passw0rd!",
        ConfirmPassword = "Passw0rd!"
    };

    // ── FullName ──────────────────────────────────────────────────────────────

    [Fact]
    public void FullName_Empty_ReturnsError()
    {
        var dto = ValidDto(); dto.FullName = "";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Fact]
    public void FullName_ExceedsMaxLength_ReturnsError()
    {
        var dto = ValidDto(); dto.FullName = new string('A', 101);
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    // ── Email ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Email_Empty_ReturnsError()
    {
        var dto = ValidDto(); dto.Email = "";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Email_InvalidFormat_ReturnsError()
    {
        var dto = ValidDto(); dto.Email = "not-an-email";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    // ── Password ──────────────────────────────────────────────────────────────

    [Fact]
    public void Password_TooShort_ReturnsError()
    {
        var dto = ValidDto(); dto.Password = "Ab1!"; dto.ConfirmPassword = "Ab1!";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Password_NoUppercase_ReturnsError()
    {
        var dto = ValidDto(); dto.Password = "passw0rd!"; dto.ConfirmPassword = "passw0rd!";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Password_NoLowercase_ReturnsError()
    {
        var dto = ValidDto(); dto.Password = "PASSW0RD!"; dto.ConfirmPassword = "PASSW0RD!";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Password_NoDigit_ReturnsError()
    {
        var dto = ValidDto(); dto.Password = "Password!"; dto.ConfirmPassword = "Password!";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Password_NoSpecialCharacter_ReturnsError()
    {
        var dto = ValidDto(); dto.Password = "Passw0rdd"; dto.ConfirmPassword = "Passw0rdd";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    // ── ConfirmPassword ───────────────────────────────────────────────────────

    [Fact]
    public void ConfirmPassword_MismatchesPassword_ReturnsError()
    {
        var dto = ValidDto(); dto.ConfirmPassword = "Different1!";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "ConfirmPassword");
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_ValidDto_PassesWithNoErrors()
    {
        var result = _sut.Validate(ValidDto());
        result.IsValid.Should().BeTrue();
    }
}
