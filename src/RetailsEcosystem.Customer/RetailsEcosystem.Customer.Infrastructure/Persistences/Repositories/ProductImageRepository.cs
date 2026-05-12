using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _db;

        public ProductImageRepository(AppDbContext db) => _db = db;

        public Task AddImageRangeAsync(int productId, List<string> imageUrls)
        {
            _db.ProductImages.AddRange(imageUrls.Select(url => new ProductImage
            {
                Product = _db.Products.Find(productId)
                    ?? throw new KeyNotFoundException($"Product {productId} not found."),
                Url = url
            }));
            return Task.CompletedTask;
        }

        public async Task<ProductImage?> GetByIdAsync(int imageId) =>
            await _db.ProductImages.FindAsync(imageId);

        public async Task DeleteAsync(int imageId)
        {
            var image = await _db.ProductImages.FindAsync(imageId)
                ?? throw new KeyNotFoundException($"Image {imageId} not found.");
            _db.ProductImages.Remove(image);
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId) =>
            await _db.ProductImages
                .Where(i => i.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
    }
}
