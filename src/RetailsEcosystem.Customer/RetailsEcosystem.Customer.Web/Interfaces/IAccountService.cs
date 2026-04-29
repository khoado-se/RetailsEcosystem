using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface IAccountService
    {
        Task<AuthResponseDto> LoginAsync(string email, string password);
        Task<AuthResponseDto> RegisterAsync(string fullName, string email, string password, string confirmPassword);
        Task LogoutAsync(string accessToken);
        Task<CustomerDto> GetProfileAsync(string accessToken);
        Task UpdateProfileAsync(string accessToken, UpdateProfileDto dto);
        Task ChangePasswordAsync(string accessToken, string currentPassword, string newPassword);
    }
}
