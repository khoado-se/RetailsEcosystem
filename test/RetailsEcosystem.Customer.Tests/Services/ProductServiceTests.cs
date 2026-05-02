using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Product;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;

namespace RetailsEcosystem.Customer.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_productRepoMock.Object, _categoryRepoMock.Object);
    }

    // ── GetAllProductAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllProductAsync_NoSearch_ReturnsPagedResult()
    {
        var products = new[] { new ProductBuilder().Build() };
        _productRepoMock.Setup(r => r.GetAllProductAsync(1, 8, null, null)).ReturnsAsync(products);
        _productRepoMock.Setup(r => r.GetProductCountAsync(null, false, null)).ReturnsAsync(1);

        var result = await _sut.GetAllProductAsync(new PagedRequest { PageNumber = 1, PageSize = 8 }, null);

        result.Items.Should().HaveCount(1);
        result.TotalPage.Should().Be(1);
    }

    [Fact]
    public async Task GetAllProductAsync_WithSearch_PassesSearchToRepo()
    {
        _productRepoMock.Setup(r => r.GetAllProductAsync(1, 8, null, "laptop")).ReturnsAsync([]);
        _productRepoMock.Setup(r => r.GetProductCountAsync(null, false, "laptop")).ReturnsAsync(0);

        await _sut.GetAllProductAsync(new PagedRequest { PageNumber = 1, PageSize = 8, Search = "laptop" }, null);

        _productRepoMock.Verify(r => r.GetAllProductAsync(1, 8, null, "laptop"), Times.Once);
    }

    [Fact]
    public async Task GetAllProductAsync_WithCategoryId_PassesCategoryToRepo()
    {
        _productRepoMock.Setup(r => r.GetAllProductAsync(1, 8, 5, null)).ReturnsAsync([]);
        _productRepoMock.Setup(r => r.GetProductCountAsync(5, false, null)).ReturnsAsync(0);

        await _sut.GetAllProductAsync(new PagedRequest { PageNumber = 1, PageSize = 8 }, 5);

        _productRepoMock.Verify(r => r.GetAllProductAsync(1, 8, 5, null), Times.Once);
    }

    // ── FindProductByIdAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task FindProductByIdAsync_NotFound_ReturnsNull()
    {
        _productRepoMock.Setup(r => r.GetProductByIdAsync(99)).ReturnsAsync((Product?)null);

        var result = await _sut.FindProductByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task FindProductByIdAsync_Found_MapsFirstImageToImageUrl()
    {
        var product = new ProductBuilder().WithId(1).WithImage("http://img1.jpg").WithImage("http://img2.jpg").Build();
        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        var result = await _sut.FindProductByIdAsync(1);

        result!.ImageUrl.Should().Be("http://img1.jpg");
        result.ImageUrls.Should().HaveCount(2);
    }

    // ── DeleteProductAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteProductAsync_NotFound_ThrowsException()
    {
        _productRepoMock.Setup(r => r.CheckExist(99)).ReturnsAsync(false);

        var act = () => _sut.DeleteProductAsync(99);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*not found*");
    }

    // ── CreateProductAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateProductAsync_InvalidCategory_PropagatesException()
    {
        _categoryRepoMock.Setup(r => r.GetCategoryByIdAsync(999))
            .ThrowsAsync(new KeyNotFoundException("Category not found"));

        var act = () => _sut.CreateProductAsync(new CreateProductDto { Name = "New", CategoryId = 999, Price = 10 });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
