using FluentAssertions;
using RetailsEcosystem.Customer.Application.Validators;
using RetailsEcosystem.Customer.Shared.DTOs.Category;

namespace RetailsEcosystem.Customer.Tests.Validators;

public class UpdateCategoryDtoValidatorTests
{
    private readonly UpdateCategoryDtoValidator _sut = new();

    private static UpdateCategoryDto ValidDto() => new() { Id = 1, Name = "Electronics" };

    // ── Name ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Name_Empty_ReturnsError()
    {
        var dto = ValidDto(); dto.Name = "";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Name_ExceedsMaxLength_ReturnsError()
    {
        var dto = ValidDto(); dto.Name = new string('A', 101);
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_ValidDto_PassesWithNoErrors()
    {
        var result = _sut.Validate(ValidDto());
        result.IsValid.Should().BeTrue();
    }
}
