using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Web.Attributes
{
    /// <summary>
    /// Global pre-action filter that checks the stored JWT expiry claim before every request.
    /// If the stored access token has expired, the user is signed out and redirected to login
    /// — preventing a 401 from the API and showing a clear "session expired" flow instead.
    /// </summary>
    public class TokenExpiryFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                // Skip the expiry check for Account actions (Login, Register, Logout)
                // so we don't intercept the logout flow or create redirect loops.
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (descriptor?.ControllerName == "Account")
                {
                    await next();
                    return;
                }

                var expiresAt = user.FindFirstValue("token_expires_at");
                if (expiresAt is not null &&
                    DateTime.TryParse(expiresAt, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiry) &&
                    expiry <= DateTime.UtcNow)
                {
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Result = new RedirectToActionResult(
                        "Login", "Account",
                        new { returnUrl = context.HttpContext.Request.Path });
                    return;
                }
            }

            await next();
        }
    }
}
