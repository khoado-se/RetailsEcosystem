using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetailsEcosystem.Customer.Infrastructure.Identity.DataSeed;

namespace RetailsEcosystem.Customer.API.Extensions
{
    public static class AppSeedExtensions
    {
        public static async Task SeedIdentityAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                await IdentitySeed.SeedAsync(services);
            }
            catch (Exception ex)
            {
                // In production, use ILogger
                Console.WriteLine($"An error occurred while seeding Identity: {ex.Message}");
            }
        }
    }
}
