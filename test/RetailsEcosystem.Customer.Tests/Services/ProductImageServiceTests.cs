using FluentAssertions;
using Moq;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Tests.Services;

public class ProductImageServiceTests
{
    private readonly Mock<IProductImageRepository> _imageRepoMock = new();
    private readonly ProductImageService _sut;

    public ProductImageServiceTests()
    {
        _sut = new ProductImageService(_imageRepoMock.Object);
    }

    [Fact]
    public async Task DeleteImageAsync_NotFound_ThrowsKeyNotFoundException()
    {
        _imageRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ProductImage?)null);

        var act = () => _sut.DeleteImageAsync(99);

        await act.Should().ThrowAsync<KeyNotFoundException>()
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
}
