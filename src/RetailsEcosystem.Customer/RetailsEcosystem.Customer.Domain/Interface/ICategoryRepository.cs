using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(int pageNumber, int pageSize);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<int> GetTotalCategoriesAsync();
        Task<int> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int categoryId);
    }
}
