using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);

        /// <summary>Revokes ALL refresh tokens for a user — used on logout.</summary>
        Task RevokeAllForUserAsync(string userId);

        Task SaveChangesAsync();
    }
}
