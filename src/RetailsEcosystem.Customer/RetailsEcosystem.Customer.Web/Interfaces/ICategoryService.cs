using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAllAsync(PagedRequest? pagedRequest = null);
        Task<CategoryDto?> GetByIdAsync(int id);
    }
}
