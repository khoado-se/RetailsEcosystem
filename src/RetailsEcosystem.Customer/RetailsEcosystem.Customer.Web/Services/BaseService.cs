using System.Text.Json;

namespace RetailsEcosystem.Customer.Web.Services
{
    public abstract class BaseService
    {
        protected readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        protected BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        protected async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _options);
        }
    }
}
