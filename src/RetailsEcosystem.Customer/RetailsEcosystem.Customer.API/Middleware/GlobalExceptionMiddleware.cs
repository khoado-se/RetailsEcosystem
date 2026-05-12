using System.Net;
using System.Text.Json;
using RetailsEcosystem.Customer.Application.Exceptions;

namespace RetailsEcosystem.Customer.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var (statusCode, title) = exception switch
            {
                NotFoundException           => (HttpStatusCode.NotFound, "Not Found"),
                ConflictException           => (HttpStatusCode.Conflict, "Conflict"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
                InvalidOperationException   => (HttpStatusCode.BadRequest, "Bad Request"),
                ArgumentException           => (HttpStatusCode.BadRequest, "Bad Request"),
                KeyNotFoundException        => (HttpStatusCode.NotFound, "Not Found"),
                _                           => (HttpStatusCode.InternalServerError, "Internal Server Error")
            };

            context.Response.StatusCode = (int)statusCode;

            var isDev = _env.IsDevelopment();
            var isServerError = statusCode == HttpStatusCode.InternalServerError;

            var problem = new
            {
                type = $"https://httpstatuses.com/{(int)statusCode}",
                title,
                status = (int)statusCode,
                detail = isServerError && !isDev ? "An unexpected error occurred." : exception.Message,
                traceId = context.TraceIdentifier,
                stackTrace = isDev ? exception.StackTrace : null
            };

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
