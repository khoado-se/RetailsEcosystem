using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using RetailsEcosystem.Customer.Web.Helpers;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Models.Account;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET /account/login
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST /account/login
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var (auth, refreshToken) = await _accountService.LoginAsync(model.Email, model.Password);
                var profile = await _accountService.GetProfileAsync(auth.AccessToken);
                await SignInAsync(auth.AccessToken, auth.User.FullName, auth.User.Email, auth.User.Roles,
                    refreshToken, avatarUrl: profile.AvatarUrl);
                return RedirectToLocal(model.ReturnUrl);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                                                || ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET /account/register
        [HttpGet("register")]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST /account/register
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var (auth, refreshToken) = await _accountService.RegisterAsync(
                    model.FullName, model.Email, model.Password, model.ConfirmPassword);
                await SignInAsync(auth.AccessToken, auth.User.FullName, auth.User.Email, auth.User.Roles, refreshToken);
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. The email may already be in use.");
                return View(model);
            }
        }

        // GET /account/profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var token = User.FindFirstValue("access_token")!;
            var customer = await _accountService.GetProfileAsync(token);
            var model = new ProfileViewModel
            {
                FullName    = customer.FullName,
                Email       = User.FindFirstValue(ClaimTypes.Email) ?? "",
                AvatarUrl   = customer.AvatarUrl,
                Address     = customer.Address,
                PhoneNumber = customer.PhoneNumber,
                DateOfBirth = customer.DateOfBirth
            };
            return View(model);
        }

        // POST /account/profile
        [HttpPost("profile")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = User.FindFirstValue("access_token")!;
            try
            {
                await _accountService.UpdateProfileAsync(token, new UpdateProfileDto
                {
                    FullName = model.FullName,
                    AvatarUrl = model.AvatarUrl,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = model.DateOfBirth
                });

                // Refresh name in cookie, preserving the existing refresh token
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                var existingRefreshToken = User.FindFirstValue("refresh_token");
                await SignInAsync(token, model.FullName, email, roles, existingRefreshToken,
                    model.PhoneNumber, model.AvatarUrl);

                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile. Please try again.");
                return View(model);
            }
        }

        // GET /account/change-password
        [HttpGet("change-password")]
        [Authorize]
        public IActionResult ChangePassword() => View();

        // POST /account/change-password
        [HttpPost("change-password")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = User.FindFirstValue("access_token")!;
            try
            {
                await _accountService.ChangePasswordAsync(token, model.CurrentPassword, model.NewPassword);
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction(nameof(Profile));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return View(model);
            }
        }

        // POST /account/profile/avatar
        [HttpPost("profile/avatar")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { error = "No file provided." });

            var token = User.FindFirstValue("access_token")!;
            try
            {
                var url = await _accountService.UploadAvatarAsync(token, file);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var fullName  = User.FindFirstValue(ClaimTypes.Name)!;
                var email     = User.FindFirstValue(ClaimTypes.Email)!;
                var roles     = User.Claims.Where(c => c.Type == ClaimTypes.Role)
                                           .Select(c => c.Value).ToList();
                var refreshTk = User.FindFirstValue("refresh_token");
                var phone     = User.FindFirstValue("phone_number");
                await SignInAsync(token, fullName, email, roles, refreshTk, phone, url);

                return Ok(new { url });
            }
            catch
            {
                return StatusCode(500, new { error = "Failed to upload photo. Please try again." });
            }
        }

        // POST /account/logout
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var token = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                // Best-effort: revoke server-side refresh token. Don't block logout if the call fails
                // (token may already be expired, network error, etc.)
                try { await _accountService.LogoutAsync(token); } catch { }
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task SignInAsync(
            string accessToken,
            string fullName,
            string email,
            IEnumerable<string> roles,
            string? refreshToken = null,
            string? phoneNumber = null,
            string? avatarUrl = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name,  fullName),
                new(ClaimTypes.Email, email),
                new("access_token",   accessToken)
            };

            var expiry = JwtHelper.GetTokenExpiry(accessToken);
            if (expiry.HasValue)
                claims.Add(new("token_expires_at", expiry.Value.ToString("O")));

            if (refreshToken is not null)
                claims.Add(new("refresh_token", refreshToken));

            if (!string.IsNullOrWhiteSpace(phoneNumber))
                claims.Add(new("phone_number", phoneNumber));

            if (!string.IsNullOrWhiteSpace(avatarUrl))
                claims.Add(new("avatar_url", avatarUrl));

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
