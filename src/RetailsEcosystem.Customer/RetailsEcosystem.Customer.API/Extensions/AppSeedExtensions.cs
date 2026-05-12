using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetailsEcosystem.Customer.Infrastructure.Identity.DataSeed;
using RetailsEcosystem.Customer.Infrastructure.Persistences;

namespace RetailsEcosystem.Customer.API.Extensions
{
    public static class AppSeedExtensions
    {
        public static async Task SeedIdentityAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var env = services.GetRequiredService<IHostEnvironment>();

            try
            {
                if (!env.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
                {
                    var db = services.GetRequiredService<AppDbContext>();
                    await db.Database.MigrateAsync();
                }

                await IdentitySeed.SeedAsync(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during startup: {ex.Message}");
            }
        }
    }
}
