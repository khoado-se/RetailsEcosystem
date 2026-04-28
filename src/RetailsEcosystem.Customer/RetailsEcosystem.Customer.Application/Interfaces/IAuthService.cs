using RetailsEcosystem.Customer.Shared.DTOs.Auth;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user with the Customer role.
        /// Returns the access token + user info; refresh token is returned separately
        /// so the caller (controller) can set it as an httpOnly cookie.
        /// </summary>
        Task<(AuthResponseDto response, string refreshToken)> RegisterAsync(RegisterDto dto);

        /// <summary>
        /// Validates credentials, checks IsActive, and issues a new token pair.
        /// </summary>
        Task<(AuthResponseDto response, string refreshToken)> LoginAsync(LoginDto dto);

        /// <summary>
        /// Validates the incoming refresh token, rotates it (issues a new one),
        /// and returns a fresh access token.
        /// </summary>
        Task<(AuthResponseDto response, string newRefreshToken)> RefreshTokenAsync(string rawRefreshToken);

        /// <summary>
        /// Revokes all refresh tokens for the user identified by the given raw token.
        /// </summary>
        Task LogoutAsync(string rawRefreshToken);

        /// <summary>Returns the current user's full profile.</summary>
        Task<MeResponseDto> GetMeAsync(string userId);
    }
}
