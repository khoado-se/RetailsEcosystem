using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
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
                var auth = await _accountService.LoginAsync(model.Email, model.Password);
                await SignInAsync(auth.AccessToken, auth.User.FullName, auth.User.Email, auth.User.Roles);
                return RedirectToLocal(model.ReturnUrl);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                                                || ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
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
                var auth = await _accountService.RegisterAsync(
                    model.FullName, model.Email, model.Password, model.ConfirmPassword);
                await SignInAsync(auth.AccessToken, auth.User.FullName, auth.User.Email, auth.User.Roles);
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
                FullName = customer.FullName,
                AvatarUrl = customer.AvatarUrl,
                Address = customer.Address,
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

                // Refresh name in cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                var email = User.FindFirstValue(ClaimTypes.Email)!;
                await SignInAsync(token, model.FullName, email, roles);

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

        // POST /account/logout
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var token = User.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(token))
                await _accountService.LogoutAsync(token);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task SignInAsync(string accessToken, string fullName, string email, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, fullName),
                new(ClaimTypes.Email, email),
                new("access_token", accessToken)
            };
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
