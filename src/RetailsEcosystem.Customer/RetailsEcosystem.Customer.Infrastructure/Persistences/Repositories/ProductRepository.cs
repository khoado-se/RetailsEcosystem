using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product.Id;
        }

        public Task EditProductAsync(Product product)
        {
            _context.Products.Update(product);
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductAsync(int pageNumber, int pageSize, int? categogyId)
        {
            var query = _context.Products.Include(p => p.Category);
            IEnumerable<Product> products = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => !categogyId.HasValue || p.Category.Id == categogyId.Value) // no category or filter by category
                    .AsNoTracking()
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p=>p.Category)
                .FirstOrDefaultAsync(p=> p.Id == productId);
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task RemoveProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckExist(int productId)
        {
            return _context.Products.Any(p => p.Id == productId);
        }
    }
}
