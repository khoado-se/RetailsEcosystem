using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(PagedRequest pagedRequest, int? categoryId);
        Task<ProductDto> GetByIdAsync(int productId);
    }
}
