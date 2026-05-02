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

        public async Task<int> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category.Id;
        }

        public async Task UpdateAsync(Category category)
        {
            var tracked = await _context.Categories.FindAsync(category.Id)
                ?? throw new KeyNotFoundException($"Category {category.Id} not found.");

            tracked.Name = category.Name;
            tracked.Description = category.Description;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int cateogryId)
        {
            var category = await _context.Categories.FindAsync(cateogryId) 
                ?? throw new KeyNotFoundException($"Category {cateogryId} not found.");
            
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int pageNumber, int pageSize)
        {
            var categories = await _context.Categories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            return category;
        }

        public async Task<int> GetTotalCategoriesAsync()
        {
            return await _context.Categories.CountAsync();
        }
    }
}
