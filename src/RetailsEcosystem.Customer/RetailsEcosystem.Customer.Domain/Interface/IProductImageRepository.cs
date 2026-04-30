using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface IProductImageRepository
    {
        Task AddImageRangeAsync(int productId, List<string> imageUrls);
        Task<ProductImage?> GetByIdAsync(int imageId);
        Task DeleteAsync(int imageId);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
    }
}
