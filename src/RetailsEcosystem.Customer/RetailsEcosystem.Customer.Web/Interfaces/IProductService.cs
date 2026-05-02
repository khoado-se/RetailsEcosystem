using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(PagedRequest pagedRequest, int? categoryId, string? search = null);
        Task<ProductDto> GetByIdAsync(int productId);

        Task<PagedResult<ProductDto>> GetFeaturedProductsAsync(PagedRequest request);
    }
}
