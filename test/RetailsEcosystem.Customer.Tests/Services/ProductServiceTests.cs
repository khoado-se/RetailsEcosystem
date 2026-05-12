using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Application.Mappings;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Product;
using RetailsEcosystem.Customer.Tests.Helpers.Builders;

namespace RetailsEcosystem.Customer.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        MappingConfig.Configure();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _sut = new ProductService(_productRepoMock.Object, _categoryRepoMock.Object, _unitOfWorkMock.Object);
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

    // ── UpdateProductAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateProductAsync_ValidDto_CallsRepositoryUpdate()
    {
        var category = new CategoryBuilder().Build();
        _productRepoMock.Setup(r => r.CheckExist(1)).ReturnsAsync(true);
        _categoryRepoMock.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync(category);

        await _sut.UpdateProductAsync(new UpdateProductDto { Id = 1, Name = "Updated", CategoryId = 1, Price = 50 });

        _productRepoMock.Verify(r => r.EditProductAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ProductNotFound_ThrowsException()
    {
        _productRepoMock.Setup(r => r.CheckExist(99)).ReturnsAsync(false);

        var act = () => _sut.UpdateProductAsync(new UpdateProductDto { Id = 99, Name = "X", CategoryId = 1, Price = 1 });

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*not found*");
    }

    // ── GetAllProductAsync pagination ─────────────────────────────────────────

    [Fact]
    public async Task GetAllProductAsync_MultiplePages_ComputesTotalPagesCorrectly()
    {
        _productRepoMock.Setup(r => r.GetAllProductAsync(1, 8, null, null)).ReturnsAsync([]);
        _productRepoMock.Setup(r => r.GetProductCountAsync(null, false, null)).ReturnsAsync(17);

        var result = await _sut.GetAllProductAsync(new PagedRequest { PageNumber = 1, PageSize = 8 }, null);

        result.TotalPage.Should().Be(3);
    }

    // ── FindProductByIdAsync edge cases ──────────────────────────────────────

    [Fact]
    public async Task FindProductByIdAsync_ProductWithNoImages_SetsImageUrlToNull()
    {
        var product = new ProductBuilder().WithId(1).Build();
        _productRepoMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

        var result = await _sut.FindProductByIdAsync(1);

        result!.ImageUrl.Should().BeNull();
        result.ImageUrls.Should().BeEmpty();
    }

    // ── GetFeaturedProductsAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetFeaturedProductsAsync_ReturnsPagedResult()
    {
        var products = new[] { new ProductBuilder().WithId(1).Build() };
        _productRepoMock.Setup(r => r.GetFeaturedProductsAsync(1, 8)).ReturnsAsync(products);
        _productRepoMock.Setup(r => r.GetProductCountAsync(null, true, null, null)).ReturnsAsync(1);

        var result = await _sut.GetFeaturedProductsAsync(new PagedRequest { PageNumber = 1, PageSize = 8 });

        result.Items.Should().HaveCount(1);
        result.TotalPage.Should().Be(1);
    }

    [Fact]
    public async Task GetFeaturedProductsAsync_EmptyRepo_ReturnsEmptyPagedResult()
    {
        _productRepoMock.Setup(r => r.GetFeaturedProductsAsync(1, 8)).ReturnsAsync([]);
        _productRepoMock.Setup(r => r.GetProductCountAsync(null, true, null, null)).ReturnsAsync(0);

        var result = await _sut.GetFeaturedProductsAsync(new PagedRequest { PageNumber = 1, PageSize = 8 });

        result.Items.Should().BeEmpty();
        result.TotalPage.Should().Be(0);
    }
}
