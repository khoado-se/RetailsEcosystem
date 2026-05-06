using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductAsync(
            int pageNumber, int pageSize,
            int? categoryId = null, string? search = null,
            bool? isFeatured = null,
            string? sortBy = null, bool sortDesc = true);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<int> AddProductAsync(Product product);
        Task EditProductAsync(Product product);
        Task RemoveProductAsync(int productId);

        Task<int> GetProductCountAsync(int? categoryId = null, bool isFeature = false, string? search = null, bool? isFeatured = null);
        Task<bool> CheckExist(int productId);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int pageNumber, int pageSize);
    }
}
