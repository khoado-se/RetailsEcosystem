using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAllAsync(PagedRequest pagedRequest);
    }
}