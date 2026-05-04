using Microsoft.AspNetCore.WebUtilities;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly string ProductUrl = "api/Products";
        public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<PagedResult<ProductDto>> GetAllAsync(PagedRequest pagedRequest, int? categoryId, string? search = null)
        {
            var url = $"{ProductUrl}?pageNumber={pagedRequest.PageNumber}&pageSize={pagedRequest.PageSize}";

            if (categoryId.HasValue)
                url += $"&categoryId={categoryId.Value}";

            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";

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
            var query = new Dictionary<string, string?>
            {
                ["pageNumber"] = pagedRequest.PageNumber.ToString(),
                ["pageSize"] = pagedRequest.PageSize.ToString()
            };

            var url = QueryHelpers.AddQueryString(
                $"{ProductUrl}/featured",
                query
            );

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                url
            );

            return await SendAsync<PagedResult<ProductDto>>(request);
        }
    }
}
