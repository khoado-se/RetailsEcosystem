using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using RetailsEcosystem.Customer.Web.Helpers;
using RetailsEcosystem.Customer.Web.Interfaces;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Web.Attributes
{
    public class TokenExpiryFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                await next();
                return;
            }

            // Skip Account controller to avoid refresh loops on login/logout/register
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (string.Equals(descriptor?.ControllerName, "Account", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            var expiryRaw = httpContext.User.FindFirstValue("token_expires_at");
            if (expiryRaw is null ||
                !DateTime.TryParse(expiryRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiry))
            {
                await next();
                return;
            }

            if (expiry > DateTime.UtcNow)
            {
                await next();
                return;
            }

            // Token is expired — attempt silent refresh
            var storedRefreshToken = httpContext.User.FindFirstValue("refresh_token");

            if (storedRefreshToken is not null)
            {
                try
                {
                    var accountService = httpContext.RequestServices.GetRequiredService<IAccountService>();
                    var (newAuth, newRefreshToken) = await accountService.RefreshAsync(storedRefreshToken);

                    if (newAuth is not null)
                    {
                        var existingClaims = httpContext.User.Claims
                            .Where(c => c.Type != "access_token"
                                     && c.Type != "token_expires_at"
                                     && c.Type != "refresh_token")
                            .ToList();

                        var newExpiry = JwtHelper.GetTokenExpiry(newAuth.AccessToken);
                        existingClaims.Add(new Claim("access_token", newAuth.AccessToken));
                        if (newExpiry.HasValue)
                            existingClaims.Add(new Claim("token_expires_at", newExpiry.Value.ToString("O")));
                        if (newRefreshToken is not null)
                            existingClaims.Add(new Claim("refresh_token", newRefreshToken));

                        var identity = new ClaimsIdentity(
                            existingClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await httpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity),
                            new AuthenticationProperties { IsPersistent = true });

                        await next();
                        return;
                    }
                }
                catch
                {
                    // API unreachable — fall through to sign-out
                }
            }

            // Refresh failed or no refresh token — sign out
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var returnUrl = Uri.EscapeDataString(httpContext.Request.Path + httpContext.Request.QueryString);
            context.Result = new RedirectResult($"/account/login?returnUrl={returnUrl}");
        }
    }
}
