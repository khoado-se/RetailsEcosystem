using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs.ProductImage;

namespace RetailsEcosystem.Customer.Tests.Services;

public class ProductImageServiceTests
{
    private readonly Mock<IProductImageRepository> _imageRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ProductImageService _sut;

    public ProductImageServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _sut = new ProductImageService(_imageRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task DeleteImageAsync_NotFound_ThrowsNotFoundException()
    {
        _imageRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ProductImage?)null);

        var act = () => _sut.DeleteImageAsync(99);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*99*");
    }

    [Theory]
    [InlineData(
        "https://res.cloudinary.com/cloud/image/upload/v123/products/abc.jpg",
        "products/abc")]
    [InlineData(
        "https://res.cloudinary.com/cloud/image/upload/products/abc.jpg",
        "products/abc")]
    [InlineData(
        "https://res.cloudinary.com/cloud/image/upload/v999/products/folder/img.png",
        "products/folder/img")]
    [InlineData(
        "https://res.cloudinary.com/cloud/image/upload/v1/solo.jpg",
        "solo")]
    public async Task DeleteImageAsync_ExtractsCorrectCloudinaryPublicId(string url, string expectedPublicId)
    {
        _imageRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ProductImage { Id = 1, Url = url, ProductId = 10 });
        _imageRepoMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        var publicId = await _sut.DeleteImageAsync(1);

        publicId.Should().Be(expectedPublicId);
    }

    [Fact]
    public async Task DeleteImageAsync_CallsDeleteOnRepository()
    {
        _imageRepoMock.Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(new ProductImage
            {
                Id = 5,
                Url = "https://res.cloudinary.com/cloud/image/upload/v1/products/test.jpg",
                ProductId = 1
            });

        await _sut.DeleteImageAsync(5);

        _imageRepoMock.Verify(r => r.DeleteAsync(5), Times.Once);
    }

    // ── AddImageRangeAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task AddImageRangeAsync_CallsRepositoryWithUrls()
    {
        var urls = new List<string> { "https://example.com/img1.jpg", "https://example.com/img2.jpg" };
        _imageRepoMock.Setup(r => r.AddImageRangeAsync(10, urls)).Returns(Task.CompletedTask);

        await _sut.AddImageRangeAsync(10, urls);

        _imageRepoMock.Verify(r => r.AddImageRangeAsync(10, urls), Times.Once);
    }

    // ── GetByProductIdAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetByProductIdAsync_ReturnsMappedDtos()
    {
        var images = new[]
        {
            new ProductImage { Id = 1, Url = "https://example.com/img1.jpg", ProductId = 10 },
            new ProductImage { Id = 2, Url = "https://example.com/img2.jpg", ProductId = 10 }
        };
        _imageRepoMock.Setup(r => r.GetByProductIdAsync(10)).ReturnsAsync(images);

        var result = await _sut.GetByProductIdAsync(10);

        result.Should().HaveCount(2);
        result.Should().ContainSingle(i => i.Id == 1 && i.Url == "https://example.com/img1.jpg");
    }

    // ── ExtractPublicId edge case ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteImageAsync_UrlWithNoUploadSegment_ReturnsEmptyPublicId()
    {
        _imageRepoMock.Setup(r => r.GetByIdAsync(7))
            .ReturnsAsync(new ProductImage { Id = 7, Url = "https://res.cloudinary.com/cloud/image/upload/", ProductId = 1 });
        _imageRepoMock.Setup(r => r.DeleteAsync(7)).Returns(Task.CompletedTask);

        var publicId = await _sut.DeleteImageAsync(7);

        publicId.Should().BeEmpty();
    }
}
