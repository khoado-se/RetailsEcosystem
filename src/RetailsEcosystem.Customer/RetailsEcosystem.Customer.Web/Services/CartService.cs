using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public CartService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<CartDto> GetCartAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/carts");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CartDto>(json, _jsonOptions)!;
        }

        public async Task<CartDto> AddItemAsync(string accessToken, int productId, int quantity)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/carts/items");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { productId, quantity }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CartDto>(json, _jsonOptions)!;
        }

        public async Task<CartDto> UpdateItemAsync(string accessToken, int cartItemId, int quantity)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/carts/items/{cartItemId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { quantity }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CartDto>(json, _jsonOptions)!;
        }

        public async Task<CartDto> RemoveItemAsync(string accessToken, int cartItemId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/carts/items/{cartItemId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CartDto>(json, _jsonOptions)!;
        }

        public async Task ClearCartAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/carts");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
