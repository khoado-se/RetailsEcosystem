using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Controllers;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Tests.Web.Controllers;

public class HomeControllerTests
{
    private readonly Mock<IProductService> _productSvcMock = new();

    private HomeController BuildController()
    {
        var controller = new HomeController(_productSvcMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsViewWithFeaturedProducts()
    {
        var featured = new PagedResult<ProductDto>
        {
            Items      = [new ProductDto { Id = 1, Name = "Widget" }],
            TotalCount = 1,
            PageNumber = 1,
            PageSize   = 6
        };
        _productSvcMock
            .Setup(s => s.GetFeaturedProductsAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(featured);
        var sut = BuildController();

        var result = await sut.Index();

        result.Should().BeOfType<ViewResult>();
        var items = sut.ViewBag.FeatureProducts as IEnumerable<ProductDto>;
        items.Should().HaveCount(1);
    }

    [Fact]
    public void Privacy_ReturnsView()
    {
        var sut = BuildController();

        var result = sut.Privacy();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ReturnsViewWithErrorViewModel()
    {
        var sut = BuildController();

        var result = sut.Error();

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().NotBeNull();
    }
}
