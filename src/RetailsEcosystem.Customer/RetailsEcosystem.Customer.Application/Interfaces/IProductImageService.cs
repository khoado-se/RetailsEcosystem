using RetailsEcosystem.Customer.Shared.DTOs.ProductImage;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IProductImageService
    {
        Task AddImageRangeAsync(int productId, List<string> imageUrls);
        Task<string> DeleteImageAsync(int imageId);
        Task<IEnumerable<ProductImageItemDto>> GetByProductIdAsync(int productId);
    }
}
