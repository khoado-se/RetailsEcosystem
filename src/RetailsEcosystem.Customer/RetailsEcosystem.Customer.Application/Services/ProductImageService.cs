using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _productImageRepo;
        public ProductImageService(IProductImageRepository productImageRepo)
        {
            _productImageRepo = productImageRepo;
        }
        public async Task AddImageRangeAsync(int productId, List<string> imageUrls)
        {
            await _productImageRepo.AddImageRangeAsync(productId, imageUrls);
        }
    }
}
