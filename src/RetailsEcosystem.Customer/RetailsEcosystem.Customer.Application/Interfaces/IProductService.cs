using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Product;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IProductService
    {
        Task<PageResult<ProductDto>> GetAllProductAsync(PagedRequest pagedRequest);
        Task<ProductDto?> FindProductByIdAsync(int productId);
        Task<int> CreateProductAsync(CreateProductDto product);
        Task UpdateProductAsync(UpdateProductDto product);
        Task DeleteProductAsync(int productId);
    }
}
