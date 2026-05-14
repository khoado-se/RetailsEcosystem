using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Controllers;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Tests.Web.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService>  _productSvcMock  = new();
    private readonly Mock<ICategoryService> _categorySvcMock = new();

    private ProductsController BuildController()
    {
        var controller = new ProductsController(_productSvcMock.Object, _categorySvcMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    private static PagedResult<ProductDto> ProductPage() => new()
    {
        Items      = [new ProductDto { Id = 1, Name = "Widget" }],
        TotalCount = 1,
        PageNumber = 1,
        PageSize   = 10
    };

    private static PagedResult<CategoryDto> CategoryPage(string? name = null) => new()
    {
        Items      = [new CategoryDto { Id = 3, Name = name ?? "Electronics" }],
        TotalCount = 1,
        PageNumber = 1,
        PageSize   = 200
    };

    // ── ProductIndex ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ProductIndex_NoCategoryFilter_ReturnsViewWithProducts()
    {
        _productSvcMock
            .Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>(), null, null))
            .ReturnsAsync(ProductPage());
        _categorySvcMock
            .Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(CategoryPage());
        var sut = BuildController();

        var result = await sut.ProductIndex(new PagedRequest { PageNumber = 1, PageSize = 10 });

        result.Should().BeOfType<ViewResult>();
        ((object?)sut.ViewBag.CategoryId).Should().BeNull();
    }

    [Fact]
    public async Task ProductIndex_WithCategoryId_CategoryNameFoundInList()
    {
        _productSvcMock
            .Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>(), 3, null))
            .ReturnsAsync(ProductPage());
        _categorySvcMock
            .Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(CategoryPage("Electronics"));
        var sut = BuildController();

        await sut.ProductIndex(new PagedRequest { PageNumber = 1, PageSize = 10 }, categoryId: 3);

        ((string?)sut.ViewBag.SelectedCategoryName).Should().Be("Electronics");
    }

    [Fact]
    public async Task ProductIndex_CategoryNotInList_FetchesFromService()
    {
        var emptyCategories = new PagedResult<CategoryDto>
        {
            Items = [], TotalCount = 0, PageNumber = 1, PageSize = 200
        };
        _productSvcMock
            .Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>(), 99, null))
            .ReturnsAsync(ProductPage());
        _categorySvcMock
            .Setup(s => s.GetAllAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(emptyCategories);
        _categorySvcMock
            .Setup(s => s.GetByIdAsync(99))
            .ReturnsAsync(new CategoryDto { Id = 99, Name = "Rare" });
        var sut = BuildController();

        await sut.ProductIndex(new PagedRequest { PageNumber = 1, PageSize = 10 }, categoryId: 99);

        ((string?)sut.ViewBag.SelectedCategoryName).Should().Be("Rare");
    }

    // ── ProductDetails ────────────────────────────────────────────────────────

    [Fact]
    public async Task ProductDetails_ProductExists_ReturnsViewWithProduct()
    {
        _productSvcMock
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(new ProductDto { Id = 1, Name = "Gadget" });
        var sut = BuildController();

        var result = await sut.ProductDetails(1);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<ProductDto>()
            .Which.Name.Should().Be("Gadget");
        sut.ViewData["DynamicTitle"].Should().Be("Gadget");
    }

    [Fact]
    public async Task ProductDetails_ProductNotFound_Returns404()
    {
        _productSvcMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((ProductDto?)null!);
        var sut = BuildController();

        var result = await sut.ProductDetails(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
