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

        public Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            return Task.CompletedTask;
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
        }

        public async Task<IEnumerable<Product>> GetAllProductAsync(
            int pageNumber, int pageSize,
            int? categoryId = null, string? search = null,
            bool? isFeatured = null,
            string? sortBy = null, bool sortDesc = true)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p =>
                    (!categoryId.HasValue || p.Category.Id == categoryId.Value) &&
                    (string.IsNullOrWhiteSpace(search) || p.Name.Contains(search)) &&
                    (!isFeatured.HasValue || p.IsFeatured == isFeatured.Value))
                .AsNoTracking();

            query = (sortBy?.ToLower(), sortDesc) switch
            {
                ("id",          true)  => query.OrderByDescending(p => p.Id),
                ("id",          false) => query.OrderBy(p => p.Id),
                ("name",        true)  => query.OrderByDescending(p => p.Name),
                ("name",        false) => query.OrderBy(p => p.Name),
                ("price",       true)  => query.OrderByDescending(p => p.Price),
                ("price",       false) => query.OrderBy(p => p.Price),
                ("createddate", false) => query.OrderBy(p => p.CreatedDate).ThenBy(p => p.Id),
                _                      => query.OrderByDescending(p => p.CreatedDate).ThenByDescending(p => p.Id),
            };

            return await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<int> GetProductCountAsync(int? categoryId = null, bool isFeature = false, string? search = null, bool? isFeatured = null)
        {
            if (isFeature)
                return await _context.Products.CountAsync(p => p.IsFeatured);

            return await _context.Products
                .Where(p =>
                    (!categoryId.HasValue || p.Category.Id == categoryId.Value) &&
                    (string.IsNullOrWhiteSpace(search) || p.Name.Contains(search)) &&
                    (!isFeatured.HasValue || p.IsFeatured == isFeatured.Value))
                .CountAsync();
        }

        public async Task RemoveProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found.");
            _context.Products.Remove(product);
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
