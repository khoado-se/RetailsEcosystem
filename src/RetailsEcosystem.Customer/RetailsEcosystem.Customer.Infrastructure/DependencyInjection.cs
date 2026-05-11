using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Application.Services;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Infrastructure.Identity;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories;
using RetailsEcosystem.Customer.Shared.Settings;

namespace RetailsEcosystem.Customer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ── Database ──────────────────────────────────────────────────────
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                ));

            // ── ASP.NET Core Identity ─────────────────────────────────────────
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password policy
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Account lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ── JWT Authentication ────────────────────────────────────────────
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    // Eliminate the 5-minute clock skew tolerance for strict expiry
                    ClockSkew = TimeSpan.Zero
                };
            });

            // ── Existing domain services ──────────────────────────────────────
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductImageService, ProductImageService>();

            // ── Auth services ─────────────────────────────────────────────────
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // ── Customer services ─────────────────────────────────────────────
            services.AddScoped<ICustomerService, CustomerService>();

            // ── Cart services ─────────────────────────────────────────────────
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();

            // ── Order services ────────────────────────────────────────────────
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>();
            services.AddScoped<IOrderService, OrderService>();

            // ── VNPay services ────────────────────────────────────────────────
            services.Configure<VnpaySettings>(configuration.GetSection("VnpaySettings"));
            services.AddHttpClient("VNPay");
            services.AddScoped<IVnpayService, VnpayService>();

            return services;
        }
    }
}

    