using Microsoft.OpenApi;
using RetailsEcosystem.Customer.API.Filters;

namespace RetailsEcosystem.Customer.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerAuthentication(
        this IServiceCollection services)
    {
        const string swaggerJwtSchemeId = "Bearer";

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(swaggerJwtSchemeId, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Bearer. Paste the raw token only (Swagger sends it as Authorization: Bearer <token>)."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(swaggerJwtSchemeId, document)] = []
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        return services;
    }
}