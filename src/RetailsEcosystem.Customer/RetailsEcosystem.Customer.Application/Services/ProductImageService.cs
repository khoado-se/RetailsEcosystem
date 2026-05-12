using System.Text.RegularExpressions;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs.ProductImage;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _productImageRepo;

        public ProductImageService(IProductImageRepository productImageRepo)
        {
            _productImageRepo = productImageRepo;
        }

        public async Task AddImageRangeAsync(int productId, List<string> imageUrls)
        {
            await _productImageRepo.AddImageRangeAsync(productId, imageUrls);
        }

        public async Task<IEnumerable<ProductImageItemDto>> GetByProductIdAsync(int productId) =>
            (await _productImageRepo.GetByProductIdAsync(productId))
            .Select(i => new ProductImageItemDto { Id = i.Id, Url = i.Url });

        public async Task<string> DeleteImageAsync(int imageId)
        {
            var image = await _productImageRepo.GetByIdAsync(imageId)
                ?? throw new NotFoundException($"Image {imageId} not found.");

            var publicId = ExtractPublicId(image.Url);
            await _productImageRepo.DeleteAsync(imageId);
            return publicId;
        }

        // Extracts the Cloudinary public ID from a secure URL.
        // e.g. https://res.cloudinary.com/cloud/image/upload/v123/products/abc.jpg → "products/abc"
        private static string ExtractPublicId(string url)
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            var afterUpload = segments
                .SkipWhile(s => s != "upload")
                .Skip(1)
                .ToList();

            var withoutVersion = afterUpload
                .SkipWhile(s => Regex.IsMatch(s, @"^v\d+$"))
                .ToList();

            if (withoutVersion.Count == 0) return string.Empty;

            // Remove extension from the last segment
            var last = withoutVersion[^1];
            var dotIndex = last.LastIndexOf('.');
            withoutVersion[^1] = dotIndex >= 0 ? last[..dotIndex] : last;

            return string.Join("/", withoutVersion);
        }
    }
}
