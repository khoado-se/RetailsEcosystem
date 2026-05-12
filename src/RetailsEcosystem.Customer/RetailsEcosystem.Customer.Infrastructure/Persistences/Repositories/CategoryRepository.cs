using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(Category category)
        {
            var tracked = await _context.Categories.FindAsync(category.Id)
                ?? throw new KeyNotFoundException($"Category {category.Id} not found.");

            tracked.Name = category.Name;
            tracked.Description = category.Description;
        }

        public async Task DeleteAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId)
                ?? throw new KeyNotFoundException($"Category {categoryId} not found.");

            _context.Categories.Remove(category);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Categories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<int> GetTotalCategoriesAsync()
        {
            return await _context.Categories.CountAsync();
        }
    }
}
