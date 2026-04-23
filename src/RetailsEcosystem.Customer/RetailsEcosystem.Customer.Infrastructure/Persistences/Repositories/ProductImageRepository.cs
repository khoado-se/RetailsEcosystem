using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _db;
        public ProductImageRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddImageRangeAsync(int productId, List<string> imageUrls)
        {
            _db.ProductImages.AddRange(imageUrls.Select(url => new ProductImage
            {
                Product = _db.Products.Find(productId) ?? throw new Exception($"Product with id {productId} not found."),
                Url = url
            }));

            await _db.SaveChangesAsync();
        }
    }
}
