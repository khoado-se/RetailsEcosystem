using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<OrderDto> CreateOrderAsync(string accessToken, CreateOrderDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/orders");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderDto>(json, _jsonOptions)!;
        }

        public async Task<PagedResult<OrderDto>> GetOrdersAsync(string accessToken, int pageNumber, int pageSize)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get, $"api/orders?pageNumber={pageNumber}&pageSize={pageSize}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PagedResult<OrderDto>>(json, _jsonOptions)!;
        }

        public async Task<OrderDto> GetOrderByIdAsync(string accessToken, int orderId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/orders/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderDto>(json, _jsonOptions)!;
        }

        public async Task<OrderDto> CancelOrderAsync(string accessToken, int orderId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/orders/{orderId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderDto>(json, _jsonOptions)!;
        }
    }
}
