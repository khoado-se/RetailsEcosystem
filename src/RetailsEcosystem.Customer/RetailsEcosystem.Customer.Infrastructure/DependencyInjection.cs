using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Application.Service;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories;

namespace RetailsEcosystem.Customer.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                ));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }
    }
}
