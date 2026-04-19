using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Web.Interfaces;
using System.Text.Json;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string PRODUCTS_BASEURL = $"https://localhost:7035/api/Products";

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetAsync(PRODUCTS_BASEURL + $"?pageNumber={pageNumber}&pageSize={pageSize}");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer
                .Deserialize<PagedResult<ProductDto>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
    }
}
