using Microsoft.AspNetCore.Http;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface IAccountService
    {
        Task<(AuthResponseDto Response, string? RefreshToken)> LoginAsync(string email, string password);
        Task<(AuthResponseDto Response, string? RefreshToken)> RegisterAsync(string fullName, string email, string password, string confirmPassword);
        Task<(AuthResponseDto? Response, string? NewRefreshToken)> RefreshAsync(string refreshToken);
        Task LogoutAsync(string accessToken);
        Task<CustomerDto> GetProfileAsync(string accessToken);
        Task UpdateProfileAsync(string accessToken, UpdateProfileDto dto);
        Task ChangePasswordAsync(string accessToken, string currentPassword, string newPassword);
        Task<string> UploadAvatarAsync(string accessToken, IFormFile file);
    }
}
