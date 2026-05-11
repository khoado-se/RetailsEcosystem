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
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentAttempt> PaymentAttempts { get; set; }

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

            // ── Order relationships ───────────────────────────────────────────
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.User)
                      .WithMany()
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(o => o.TotalAmount).HasPrecision(18, 0);
                entity.Property(o => o.Status).HasConversion<int>();
                entity.Property(o => o.PaymentMethod)
                      .HasConversion<int>()
                      .HasDefaultValue(RetailsEcosystem.Customer.Shared.Enums.PaymentMethod.COD);
                entity.Property(o => o.PaymentStatus)
                      .HasConversion<int>()
                      .HasDefaultValue(RetailsEcosystem.Customer.Shared.Enums.PaymentStatus.Pending);
                entity.Property(o => o.VnpayTxnRef).HasMaxLength(100).IsRequired(false);
                entity.Property(o => o.VnpayTransactionNo).HasMaxLength(100).IsRequired(false);
                entity.Property(o => o.PaymentAttemptCount).HasDefaultValue(0);
                entity.Property(o => o.PaymentExpiresAt).IsRequired(false);
                entity.Property(o => o.LastPaymentAttemptAt).IsRequired(false);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(i => i.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(i => i.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(i => i.Product)
                      .WithMany()
                      .HasForeignKey(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(i => i.UnitPrice).HasPrecision(18, 0);
            });

            // ── PaymentAttempt relationships ──────────────────────────────────────
            modelBuilder.Entity<PaymentAttempt>(entity =>
            {
                entity.HasOne(a => a.Order)
                      .WithMany()
                      .HasForeignKey(a => a.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(a => a.TxnRef).IsUnique()
                      .HasDatabaseName("IX_PaymentAttempts_TxnRef");
                entity.HasIndex(a => a.OrderId)
                      .HasDatabaseName("IX_PaymentAttempts_OrderId");

                entity.Property(a => a.Status).HasConversion<int>();
            });

            // ── Performance indexes ───────────────────────────────────────────────
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId)
                .HasDatabaseName("IX_Orders_UserId");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_Status");

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.UserId, o.Status })
                .HasDatabaseName("IX_Orders_UserId_Status");

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
