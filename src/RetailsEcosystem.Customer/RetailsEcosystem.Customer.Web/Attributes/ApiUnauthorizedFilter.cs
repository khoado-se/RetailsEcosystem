using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace RetailsEcosystem.Customer.Web.Attributes
{
    /// <summary>
    /// Global exception filter that catches HTTP 401 responses thrown by any Web service
    /// that calls the API (via EnsureSuccessStatusCode). Signs the user out and redirects
    /// to login, preventing an unhandled exception page for expired/invalid tokens.
    /// </summary>
    public class ApiUnauthorizedFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is HttpRequestException ex &&
                ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                context.Result = new RedirectToActionResult(
                    "Login", "Account",
                    new { returnUrl = context.HttpContext.Request.Path });

                context.ExceptionHandled = true;
            }
        }
    }
}
