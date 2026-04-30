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

        public async Task EditProductAsync(Product product)
        {
            var tracked = await _context.Products.FindAsync(product.Id)
                ?? throw new KeyNotFoundException($"Product {product.Id} not found.");

            tracked.Name = product.Name;
            tracked.Description = product.Description;
            tracked.Price = product.Price;
            tracked.StockQuantity = product.StockQuantity;
            tracked.IsFeatured = product.IsFeatured;
            tracked.UpdatedDate = product.UpdatedDate;
            tracked.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();
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
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<int> GetProductCountAsync(int? categoryId, bool isFeature)
        {
            if (isFeature)
                return await _context.Products.CountAsync(p => p.IsFeatured);

            var query = _context.Products.AsQueryable();
            if (categoryId.HasValue)
                query = query.Where(p => p.Category.Id == categoryId.Value);

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
                .Where(p => p.IsFeatured)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsNoTracking()
                .OrderByDescending(p => p.SoldCount)
                .ThenByDescending(p => p.CreatedDate)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
