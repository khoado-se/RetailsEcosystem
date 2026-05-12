using Microsoft.AspNetCore.Identity;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CustomerDto> GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException($"User {userId} not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return MapToDto(user, roles);
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException($"User {userId} not found.");

            user.FullName = dto.FullName;
            user.AvatarUrl = dto.AvatarUrl;
            user.Address = dto.Address;
            user.PhoneNumber = dto.PhoneNumber;
            user.DateOfBirth = dto.DateOfBirth;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException($"User {userId} not found.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<PagedResult<CustomerDto>> GetAllAsync(PagedRequest request, string? search)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(lower) ||
                    (u.Email != null && u.Email.ToLower().Contains(lower)));
            }

            var total = query.Count();

            var users = query
                .OrderBy(u => u.FullName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = new List<CustomerDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                dtos.Add(MapToDto(user, roles));
            }

            return new PagedResult<CustomerDto>(dtos, request, total);
        }

        public async Task UpdateStatusAsync(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException($"User {userId} not found.");

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateAvatarAsync(string userId, string avatarUrl)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException($"User {userId} not found.");

            user.AvatarUrl = avatarUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        private static CustomerDto MapToDto(ApplicationUser user, IList<string> roles) => new()
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive,
            Roles = roles
        };
    }
}
