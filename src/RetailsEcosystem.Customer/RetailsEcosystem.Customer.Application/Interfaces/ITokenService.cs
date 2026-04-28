using System.Security.Claims;
using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    /// <summary>
    /// Pure token generation/validation — no database access.
    /// Implemented in Infrastructure so it can read JwtSettings from configuration.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Creates a signed JWT access token containing sub, email, name,
        /// jti (unique token ID for future blacklisting) and role claims.
        /// </summary>
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);

        /// <summary>
        /// Creates a cryptographically secure 64-byte random refresh token string.
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates the token signature and extracts claims WITHOUT checking expiry.
        /// Used exclusively in the refresh flow where the access token is already expired.
        /// </summary>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
