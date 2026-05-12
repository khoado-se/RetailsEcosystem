using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Product;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _productServiceMock = new();
    private readonly Mock<IValidator<CreateProductDto>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateProductDto>> _updateValidatorMock = new();
    private readonly ProductsController _sut;

    public ProductsControllerTests()
    {
        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _sut = new ProductsController(
            _productServiceMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetProducts_WhenCalled_ReturnsOkWithPagedResult()
    {
        var pagedResult = new PagedResult<ProductDto>([], new PagedRequest { PageNumber = 1, PageSize = 8 }, 0);
        _productServiceMock.Setup(s => s.GetAllProductAsync(It.IsAny<PagedRequest>(), null))
            .ReturnsAsync(pagedResult);

        var result = await _sut.GetProducts();

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetProduct_NotFound_Returns404()
    {
        _productServiceMock.Setup(s => s.FindProductByIdAsync(99)).ReturnsAsync((ProductDto?)null);

        var result = await _sut.GetProduct(99);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetProduct_WhenFound_ReturnsOkWithDto()
    {
        var dto = new ProductDto { Id = 1, Name = "Phone" };
        _productServiceMock.Setup(s => s.FindProductByIdAsync(1)).ReturnsAsync(dto);

        var result = await _sut.GetProduct(1);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task PutProduct_IdMismatch_ReturnsBadRequest()
    {
        var dto = new UpdateProductDto { Id = 5 };

        var result = await _sut.PutProduct(99, dto);

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task PutProduct_InvalidDto_ReturnsBadRequest()
    {
        var dto = new UpdateProductDto { Id = 1, Name = "", Price = -1, CategoryId = 1 };
        var failures = new[] { new ValidationFailure("Name", "Name is required.") };
        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _sut.PutProduct(1, dto);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task PutProduct_ValidRequest_ReturnsNoContent()
    {
        var dto = new UpdateProductDto { Id = 1, Name = "Updated", CategoryId = 1, Price = 10 };
        _productServiceMock.Setup(s => s.UpdateProductAsync(dto)).Returns(Task.CompletedTask);

        var result = await _sut.PutProduct(1, dto);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task PostProduct_InvalidDto_ReturnsBadRequest()
    {
        var dto = new CreateProductDto { Name = "", Price = -1, CategoryId = 1 };
        var failures = new[] { new ValidationFailure("Name", "Name is required.") };
        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _sut.PostProduct(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task PostProduct_ValidRequest_ReturnsCreatedAtAction()
    {
        _productServiceMock.Setup(s => s.CreateProductAsync(It.IsAny<CreateProductDto>())).ReturnsAsync(42);

        var result = await _sut.PostProduct(new CreateProductDto { Name = "New", CategoryId = 1, Price = 10 });

        result.Result.Should().BeOfType<CreatedAtActionResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task DeleteProduct_WhenCalled_ReturnsNoContent()
    {
        _productServiceMock.Setup(s => s.DeleteProductAsync(1)).Returns(Task.CompletedTask);

        var result = await _sut.DeleteProduct(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteProduct_WhenNotFound_Returns404()
    {
        _productServiceMock.Setup(s => s.DeleteProductAsync(99))
            .ThrowsAsync(new NotFoundException("Product is not found!"));

        var result = await _sut.DeleteProduct(99);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PutProduct_WhenNotFound_Returns404()
    {
        var dto = new UpdateProductDto { Id = 99, Name = "X", CategoryId = 1, Price = 10 };
        _productServiceMock.Setup(s => s.UpdateProductAsync(dto))
            .ThrowsAsync(new NotFoundException("Product is not found!"));

        var result = await _sut.PutProduct(99, dto);

        result.Should().BeOfType<NotFoundResult>();
    }
}
