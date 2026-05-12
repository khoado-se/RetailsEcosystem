using FluentAssertions;
using RetailsEcosystem.Customer.Application.Validators;
using RetailsEcosystem.Customer.Shared;

namespace RetailsEcosystem.Customer.Tests.Validators;

public class UpdateProductDtoValidatorTests
{
    private readonly UpdateProductDtoValidator _sut = new();

    private static UpdateProductDto ValidDto() => new()
    {
        Id = 1,
        Name = "Widget",
        Price = 9.99m,
        CategoryId = 1
    };

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
        var dto = ValidDto(); dto.Name = new string('A', 201);
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    // ── Price ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Price_Zero_ReturnsError()
    {
        var dto = ValidDto(); dto.Price = 0;
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Price_Negative_ReturnsError()
    {
        var dto = ValidDto(); dto.Price = -1;
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    // ── CategoryId ────────────────────────────────────────────────────────────

    [Fact]
    public void CategoryId_Zero_ReturnsError()
    {
        var dto = ValidDto(); dto.CategoryId = 0;
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_ValidDto_PassesWithNoErrors()
    {
        var result = _sut.Validate(ValidDto());
        result.IsValid.Should().BeTrue();
    }
}
