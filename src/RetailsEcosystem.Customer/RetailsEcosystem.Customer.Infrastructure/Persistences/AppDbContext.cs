using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Infrastructure.Persistences.DataSeed;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences
{
    /// <summary>
    /// Inherits IdentityDbContext to get all ASP.NET Identity tables (AspNetUsers,
    /// AspNetRoles, AspNetUserRoles, etc.) while keeping the existing domain DbSets.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Must call base first — Identity uses it to configure its own tables
            base.OnModelCreating(modelBuilder);

            // ── Domain entity relationships ───────────────────────────────────
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Cascade);

            // ── RefreshToken configuration ────────────────────────────────────
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                // Fast lookup by token string
                entity.HasIndex(rt => rt.Token).IsUnique();

                entity.HasOne(rt => rt.User)
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Cart relationships ────────────────────────────────────────────
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.UserId).IsUnique();
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasOne(i => i.Cart)
                      .WithMany(c => c.Items)
                      .HasForeignKey(i => i.CartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Product)
                      .WithMany()
                      .HasForeignKey(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(i => i.UnitPrice).HasPrecision(18, 2);
            });

            // ── Existing data seed ────────────────────────────────────────────
            modelBuilder.SeedCategories();
            modelBuilder.SeedProducts();
            modelBuilder.SeedProductImages();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
