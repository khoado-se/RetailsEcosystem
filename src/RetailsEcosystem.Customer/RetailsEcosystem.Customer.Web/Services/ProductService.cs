using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly string ProductUrl = "api/Products";
        public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<PagedResult<ProductDto>> GetAllAsync(PagedRequest pagedRequest, int? categoryId)
        {

            var url = $"{ProductUrl}?pageNumber={pagedRequest.PageNumber}&pageSize={pagedRequest.PageSize}";

            if (categoryId.HasValue)
            {
                url += $"&categoryId={categoryId.Value}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            return await SendAsync<PagedResult<ProductDto>>(request);
        }

        public async Task<ProductDto> GetByIdAsync(int productId)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get, 
                $"{ProductUrl}/{productId}"
            );

            return await SendAsync<ProductDto>(request);
        }

        public async Task<PagedResult<ProductDto>> GetFeaturedProductsAsync(PagedRequest pagedRequest)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{ProductUrl}/featured"
            );

            return await SendAsync<PagedResult<ProductDto>>(request);
        }
    }
}
