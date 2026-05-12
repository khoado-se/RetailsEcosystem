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
using RetailsEcosystem.Customer.Shared.DTOs.Category;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock = new();
    private readonly Mock<IValidator<CreateCategoryDto>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateCategoryDto>> _updateValidatorMock = new();
    private readonly CategoriesController _sut;

    public CategoriesControllerTests()
    {
        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _sut = new CategoriesController(
            _categoryServiceMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task GetCategories_WhenCalled_ReturnsOkWithPagedResult()
    {
        var pagedResult = new PagedResult<CategoryDto>([], new PagedRequest { PageNumber = 1, PageSize = 10 }, 0);
        _categoryServiceMock.Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>())).ReturnsAsync(pagedResult);

        var result = await _sut.GetCategories();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateCategory_IdMismatch_ReturnsBadRequest()
    {
        var dto = new UpdateCategoryDto { Id = 5, Name = "Test" };

        var result = await _sut.UpdateCategory(99, dto);

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task UpdateCategory_InvalidDto_ReturnsBadRequest()
    {
        var dto = new UpdateCategoryDto { Id = 3, Name = "" };
        var failures = new[] { new ValidationFailure("Name", "Name is required.") };
        _updateValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _sut.UpdateCategory(3, dto);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateCategory_ValidRequest_ReturnsOk()
    {
        var dto = new UpdateCategoryDto { Id = 3, Name = "Updated" };
        var updatedDto = new CategoryDto { Id = 3, Name = "Updated" };
        _categoryServiceMock.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(updatedDto);

        var result = await _sut.UpdateCategory(3, dto);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(updatedDto);
    }

    [Fact]
    public async Task DeleteCategory_WhenCalled_ReturnsNoContent()
    {
        _categoryServiceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

        var result = await _sut.DeleteCategory(1);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task CreateCategory_InvalidDto_ReturnsBadRequest()
    {
        var dto = new CreateCategoryDto { Name = "" };
        var failures = new[] { new ValidationFailure("Name", "Name is required.") };
        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await _sut.CreateCategory(dto);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedAtAction()
    {
        var dto = new CreateCategoryDto { Name = "New Category", Description = "Desc" };
        var created = new CategoryDto { Id = 10, Name = "New Category" };
        _categoryServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _sut.CreateCategory(dto);

        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task DeleteCategory_WhenNotFound_Returns404()
    {
        _categoryServiceMock.Setup(s => s.DeleteAsync(99))
            .ThrowsAsync(new NotFoundException("Category not found."));

        var result = await _sut.DeleteCategory(99);

        result.Should().BeOfType<NotFoundResult>();
    }
}
