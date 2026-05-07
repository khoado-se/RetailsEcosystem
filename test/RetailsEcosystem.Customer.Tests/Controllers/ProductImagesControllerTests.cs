using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailsEcosystem.Customer.API.Controllers;
using RetailsEcosystem.Customer.API.Services;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs.ProductImage;

namespace RetailsEcosystem.Customer.Tests.Controllers;

public class ProductImagesControllerTests
{
    private readonly Mock<IFileStorageService> _fileServiceMock = new();
    private readonly Mock<IProductImageService> _imageServiceMock = new();
    private readonly ProductImagesController _sut;

    public ProductImagesControllerTests()
    {
        _sut = new ProductImagesController(_fileServiceMock.Object, _imageServiceMock.Object);
    }

    // ── OnPostUploadAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task OnPostUploadAsync_ValidFiles_ReturnsOkWithUrls()
    {
        var urls = new List<string> { "https://cdn.example.com/img1.jpg" };
        _fileServiceMock.Setup(s => s.SaveFilesAsync(It.IsAny<List<IFormFile>>()))
            .ReturnsAsync(urls);
        _imageServiceMock.Setup(s => s.AddImageRangeAsync(1, urls))
            .Returns(Task.CompletedTask);

        var result = await _sut.OnPostUploadAsync(1, []);

        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }

    // ── GetByProduct ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByProduct_WhenCalled_ReturnsOkWithImageList()
    {
        var images = new List<ProductImageItemDto>
        {
            new() { Id = 1, Url = "https://cdn.example.com/img1.jpg" }
        };
        _imageServiceMock.Setup(s => s.GetByProductIdAsync(1))
            .ReturnsAsync(images);

        var result = await _sut.GetByProduct(1);

        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(images);
    }

    // ── DeleteImage ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteImage_WhenFound_ReturnsNoContent()
    {
        const string publicId = "products/img1";
        _imageServiceMock.Setup(s => s.DeleteImageAsync(1)).ReturnsAsync(publicId);
        _fileServiceMock.Setup(s => s.DeleteFileAsync(publicId)).Returns(Task.CompletedTask);

        var result = await _sut.DeleteImage(1);

        result.Should().BeOfType<NoContentResult>();
        _fileServiceMock.Verify(s => s.DeleteFileAsync(publicId), Times.Once);
    }

    [Fact]
    public async Task DeleteImage_WhenNotFound_Returns404()
    {
        _imageServiceMock.Setup(s => s.DeleteImageAsync(99))
            .ThrowsAsync(new KeyNotFoundException("Image 99 not found."));

        var result = await _sut.DeleteImage(99);

        result.Should().BeOfType<NotFoundResult>();
    }
}
