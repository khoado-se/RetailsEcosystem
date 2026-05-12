using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RetailsEcosystem.Customer.API.Services;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using System.Data.Common;

namespace RetailsEcosystem.Customer.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Create an open SQLite in-memory connection kept alive for the factory lifetime.
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            // Build SQLite options OUTSIDE of AddDbContext to avoid registering
            // SQLite infrastructure services into the application service collection
            // alongside the already-registered SQL Server services (which would trigger
            // EF Core's "multiple providers" guard).
            var sqliteOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            // Remove AppDbContext registered by AddInfrastructure (SQL Server).
            // Keep DbConnection as a singleton so the open connection outlives scopes.
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbConnection>();
            services.AddSingleton<DbConnection>(connection);

            // Factory-based registration bypasses AddDbContext so EF Core does NOT
            // register Sqlite infrastructure into the application DI — the context
            // builds its own internal provider from sqliteOptions only.
            services.AddScoped<AppDbContext>(_ => new AppDbContext(sqliteOptions));

            // Replace IFileStorageService to prevent the Cloudinary singleton factory
            // from throwing when Cloudinary credentials are absent in test config.
            services.RemoveAll<IFileStorageService>();
            services.AddScoped<IFileStorageService, FakeFileStorageService>();

            // Create schema BEFORE SeedIdentityAsync runs (which happens in Program.cs
            // before app.Run()). Using the same connection ensures the schema created
            // here is visible to the seeder and to all subsequent test requests.
            using var db = new AppDbContext(sqliteOptions);
            db.Database.EnsureCreated();
        });
    }
}
