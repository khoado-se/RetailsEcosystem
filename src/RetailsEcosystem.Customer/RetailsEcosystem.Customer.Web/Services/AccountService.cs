using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AccountService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<(AuthResponseDto Response, string? RefreshToken)> LoginAsync(string email, string password)
        {
            var body = JsonContent.Create(new { email, password });
            var response = await _httpClient.PostAsync("api/auth/login", body);

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                string message;
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    message = doc.RootElement.TryGetProperty("detail", out var d)
                        ? d.GetString() ?? "Invalid email or password."
                        : "Invalid email or password.";
                }
                catch
                {
                    message = "Invalid email or password.";
                }
                throw new HttpRequestException(message, null, response.StatusCode);
            }

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions)!;
            return (dto, ExtractRefreshTokenFromResponse(response));
        }

        public async Task<(AuthResponseDto Response, string? RefreshToken)> RegisterAsync(string fullName, string email, string password, string confirmPassword)
        {
            var body = JsonContent.Create(new { fullName, email, password, confirmPassword });
            var response = await _httpClient.PostAsync("api/auth/register", body);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions)!;
            return (dto, ExtractRefreshTokenFromResponse(response));
        }

        public async Task<(AuthResponseDto? Response, string? NewRefreshToken)> RefreshAsync(string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
            request.Headers.Add("Cookie", $"refreshToken={refreshToken}");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return (null, null);

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions);
            return (dto, ExtractRefreshTokenFromResponse(response));
        }

        public async Task LogoutAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            await _httpClient.SendAsync(request);
        }

        public async Task<CustomerDto> GetProfileAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/customers/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CustomerDto>(json, _jsonOptions)!;
        }

        public async Task UpdateProfileAsync(string accessToken, UpdateProfileDto dto)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/customers/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(dto),
                Encoding.UTF8,
                "application/json");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task ChangePasswordAsync(string accessToken, string currentPassword, string newPassword)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/customers/me/password");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { currentPassword, newPassword }),
                Encoding.UTF8,
                "application/json");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> UploadAvatarAsync(string accessToken, IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(stream), "file", file.FileName);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/customers/me/avatar");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("url").GetString()!;
        }

        private static string? ExtractRefreshTokenFromResponse(HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues("Set-Cookie", out var cookies))
                return null;

            foreach (var cookie in cookies)
            {
                if (!cookie.StartsWith("refreshToken=", StringComparison.OrdinalIgnoreCase))
                    continue;
                var value = cookie.Split(';')[0]["refreshToken=".Length..].Trim();
                return string.IsNullOrEmpty(value) ? null : value;
            }
            return null;
        }
    }
}
