using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
            IEnumerable<Product> products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .Where(p => !categogyId.HasValue || p.Category.Id == categogyId.Value) // no category or filter by category
                    .AsNoTracking()
                    .OrderByDescending(p => p.CreatedDate)
                    .ThenByDescending(p => p.Id)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<int> GetProductCountAsync(int? categoryId, bool isFeature)
        {
            if (isFeature)
            {
                return 4; // TODO: hard code for feature product, since we only have 4 feature products
            }
            var query = _context.Products.AsQueryable();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.Category.Id == categoryId.Value);
            }
            return await query.CountAsync();
        }

        public async Task RemoveProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found.");
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckExist(int productId)
        {
            return await _context.Products.AnyAsync(p => p.Id == productId);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
