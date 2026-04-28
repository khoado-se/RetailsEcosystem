using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Infrastructure.Identity.DataSeed
{
    /// <summary>
    /// Seeds roles and the default admin account at application startup.
    /// 
    /// Design rationale: Identity passwords are hashed at runtime using a salted
    /// algorithm that changes with each run, so they CANNOT be seeded via
    /// modelBuilder.HasData(). Instead, we seed via UserManager after the app starts.
    /// </summary>
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = ["Admin", "Customer"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@retailsecosystem.com";
            const string adminPassword = "Admin@123456";

            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing is not null) return; // Already seeded

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed admin user: {errors}");
            }

            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
