using FluentAssertions;
using RetailsEcosystem.Customer.Application.Validators;
using RetailsEcosystem.Customer.Shared.DTOs.Order;

namespace RetailsEcosystem.Customer.Tests.Validators;

public class CreateOrderDtoValidatorTests
{
    private readonly CreateOrderDtoValidator _sut = new();

    private static CreateOrderDto ValidDto() => new() { ShippingAddress = "123 Main St" };

    // ── ShippingAddress ───────────────────────────────────────────────────────

    [Fact]
    public void ShippingAddress_Empty_ReturnsError()
    {
        var dto = ValidDto(); dto.ShippingAddress = "";
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "ShippingAddress");
    }

    [Fact]
    public void ShippingAddress_ExceedsMaxLength_ReturnsError()
    {
        var dto = ValidDto(); dto.ShippingAddress = new string('A', 501);
        var result = _sut.Validate(dto);
        result.Errors.Should().Contain(e => e.PropertyName == "ShippingAddress");
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_ValidDto_PassesWithNoErrors()
    {
        var result = _sut.Validate(ValidDto());
        result.IsValid.Should().BeTrue();
    }
}
