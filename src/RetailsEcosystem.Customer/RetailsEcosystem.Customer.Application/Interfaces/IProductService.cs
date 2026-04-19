using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Product;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllProductAsync(PagedRequest pagedRequest, int? categoryId);
        Task<ProductDto?> FindProductByIdAsync(int productId);
        Task<int> CreateProductAsync(CreateProductDto product);
        Task UpdateProductAsync(UpdateProductDto product);
        Task DeleteProductAsync(int productId);
        Task<PagedResult<ProductDto>> GetFeaturedProductsAsync(PagedRequest pagedRequest);
    }
}
