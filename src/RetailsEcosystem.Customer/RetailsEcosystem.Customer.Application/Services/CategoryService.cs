using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;

namespace RetailsEcosystem.Customer.Application.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<PagedResult<CategoryDto>> GetAllAsync(PagedRequest pagedRequest)
        {
            IEnumerable<Category> categories = await _categoryRepository
                .GetAllAsync(pagedRequest.PageNumber, pagedRequest.PageSize);
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            int totalCategories = await _categoryRepository.GetTotalCategoriesAsync();

            return new PagedResult<CategoryDto>(
                categoryDtos, 
                pagedRequest, 
                totalCategories
            );
        }
    }
}