using RetailsEcosystem.Customer.Shared;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(int pageNumber, int pageSize);
    }
}
