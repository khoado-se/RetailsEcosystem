using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;

namespace RetailsEcosystem.Customer.Infrastructure.Identity
{
    /// <summary>
    /// Orchestrates the full auth lifecycle: registration, login, token refresh, logout, and profile.
    /// Depends on UserManager (Identity) for user management and ITokenService for token operations.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepo,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
            _configuration = configuration;
        }

        // ── Register ─────────────────────────────────────────────────────────
        public async Task<(AuthResponseDto response, string refreshToken)> RegisterAsync(RegisterDto dto)
        {
            // Guard: duplicate email
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing is not null)
                throw new InvalidOperationException("An account with this email already exists.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            // New users always get the Customer role
            await _userManager.AddToRoleAsync(user, "Customer");

            return await BuildTokenPairAsync(user);
        }

        // ── Login ─────────────────────────────────────────────────────────────
        public async Task<(AuthResponseDto response, string refreshToken)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Your account has been disabled. Contact support.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            return await BuildTokenPairAsync(user);
        }

        // ── Refresh ───────────────────────────────────────────────────────────
        public async Task<(AuthResponseDto response, string newRefreshToken)> RefreshTokenAsync(string rawRefreshToken)
        {
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(rawRefreshToken)
                ?? throw new UnauthorizedAccessException("Refresh token not found.");

            if (!storedToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token has expired or been revoked.");

            // Rotate: revoke the old token
            storedToken.IsRevoked = true;
            await _refreshTokenRepo.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(storedToken.UserId)
                ?? throw new UnauthorizedAccessException("User not found.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account has been disabled.");

            // Issue a fresh pair
            return await BuildTokenPairAsync(user);
        }

        // ── Logout ────────────────────────────────────────────────────────────
        public async Task LogoutAsync(string rawRefreshToken)
        {
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(rawRefreshToken);
            if (storedToken is null) return; // Already gone — idempotent

            // Revoke ALL tokens for this user (logout from all devices)
            await _refreshTokenRepo.RevokeAllForUserAsync(storedToken.UserId);
            await _refreshTokenRepo.SaveChangesAsync();
        }

        // ── GetMe ─────────────────────────────────────────────────────────────
        public async Task<MeResponseDto> GetMeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            return new MeResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                Roles = roles
            };
        }

        // ── Private helpers ───────────────────────────────────────────────────
        private async Task<(AuthResponseDto response, string refreshToken)> BuildTokenPairAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var rawRefreshToken = _tokenService.GenerateRefreshToken();

            var expiryDays = int.Parse(
                _configuration["JwtSettings:RefreshTokenExpiryDays"] ?? "7");

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = rawRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(expiryDays),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepo.AddAsync(refreshTokenEntity);
            await _refreshTokenRepo.SaveChangesAsync();

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:AccessTokenExpiryMinutes"] ?? "15"));

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAt = accessTokenExpiry,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles
                }
            };

            return (response, rawRefreshToken);
        }
    }
}
