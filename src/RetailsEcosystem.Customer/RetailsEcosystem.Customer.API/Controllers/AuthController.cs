using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs.Auth;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;

        // Cookie name — constant for consistency
        private const string RefreshTokenCookieName = "refreshToken";

        public AuthController(
            IAuthService authService,
            IValidator<RegisterDto> registerValidator,
            IValidator<LoginDto> loginValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        /// <summary>
        /// Registers a new user with the Customer role.
        /// POST /api/auth/register
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var validation = await _registerValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

            var (response, refreshToken) = await _authService.RegisterAsync(dto);
            SetRefreshTokenCookie(refreshToken);
            return Ok(response);
        }

        /// <summary>
        /// Authenticates a user and returns JWT access token + sets refresh token cookie.
        /// POST /api/auth/login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var validation = await _loginValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

            var (response, refreshToken) = await _authService.LoginAsync(dto);
            SetRefreshTokenCookie(refreshToken);
            return Ok(response);
        }

        /// <summary>
        /// Issues a new access token using the refresh token from the httpOnly cookie.
        /// POST /api/auth/refresh
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token not found." });

            var (response, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);
            SetRefreshTokenCookie(newRefreshToken);
            return Ok(response);
        }

        /// <summary>
        /// Revokes all refresh tokens for the current user.
        /// POST /api/auth/logout
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            if (!string.IsNullOrEmpty(refreshToken))
                await _authService.LogoutAsync(refreshToken);

            // Clear the cookie on the client side
            Response.Cookies.Delete(RefreshTokenCookieName);
            return NoContent();
        }

        /// <summary>
        /// Returns the authenticated user's profile.
        /// GET /api/auth/me
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var me = await _authService.GetMeAsync(userId);
            return Ok(me);
        }

        // ── Private ───────────────────────────────────────────────────────────

        /// <summary>
        /// Sets the refresh token as an httpOnly, Secure, SameSite=Strict cookie.
        /// httpOnly prevents JavaScript access → protects against XSS.
        /// </summary>
        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                  // HTTPS only
                SameSite = SameSiteMode.Strict, // CSRF protection
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append(RefreshTokenCookieName, token, cookieOptions);
        }
    }
}
