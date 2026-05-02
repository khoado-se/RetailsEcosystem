using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Category;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAllAsync(PagedRequest pagedRequest);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto);
        Task DeleteAsync(int id);
    }
}