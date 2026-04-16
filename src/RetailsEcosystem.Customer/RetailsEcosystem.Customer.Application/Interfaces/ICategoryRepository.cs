using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByIdAsync(int categoryId);
    }
}
