using Mapster;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Category;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<CategoryDto>> GetAllAsync(PagedRequest pagedRequest)
        {
            IEnumerable<Category> categories = await _categoryRepository
                .GetAllAsync(pagedRequest.PageNumber, pagedRequest.PageSize);
            var categoryDtos = categories.Select(c => c.Adapt<CategoryDto>()).ToList();

            int totalCategories = await _categoryRepository.GetTotalCategoriesAsync();

            return new PagedResult<CategoryDto>(
                categoryDtos, 
                pagedRequest, 
                totalCategories
            );
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty
            };

            await _categoryRepository.CreateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Adapt<CategoryDto>();
        }

        public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto)
        {
            var category = new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description ?? string.Empty
            };

            await _categoryRepository.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Adapt<CategoryDto>();
        }

        public async Task DeleteAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}