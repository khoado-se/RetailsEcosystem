using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetByIdAsync(string userId);
        Task UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<PagedResult<CustomerDto>> GetAllAsync(PagedRequest request, string? search);
        Task UpdateStatusAsync(string userId, bool isActive);
        Task UpdateAvatarAsync(string userId, string avatarUrl);
    }
}
