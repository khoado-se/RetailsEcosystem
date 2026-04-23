namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IProductImageService
    {
        Task AddImageRangeAsync(int productId, List<string> imageUrls);
    }
}
