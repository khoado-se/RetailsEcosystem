namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface IProductImageRepository
    {
        Task AddImageRangeAsync(int productId, List<string> imageUrls);
    }
}
