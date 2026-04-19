using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var created = new DateTime(2024, 1, 1);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Smartphone", Description = "Mobile phones" },
                new Category { Id = 2, Name = "Laptop", Description = "Personal computers" },
                new Category { Id = 3, Name = "Tablet", Description = "Tablets and iPads" },
                new Category { Id = 4, Name = "Accessory", Description = "Accessories" },
                new Category { Id = 5, Name = "Smartwatch", Description = "Wearable devices" }
            );

            //Configure the relationships and constraints
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Product>().HasData(
                new
                {
                    Id = 1,
                    Name = "iPhone 15 Pro",
                    Description = "Apple flagship phone",
                    Price = 999m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 1
                },
                new
                {
                    Id = 2,
                    Name = "Samsung Galaxy S24",
                    Description = "Samsung flagship phone",
                    Price = 899m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 1
                },
                new
                {
                    Id = 3,
                    Name = "Xiaomi 14",
                    Description = "High performance phone",
                    Price = 699m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 1
                },

                new
                {
                    Id = 4,
                    Name = "MacBook Pro M3",
                    Description = "Apple laptop",
                    Price = 1999m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 2
                },
                new
                {
                    Id = 5,
                    Name = "Dell XPS 13",
                    Description = "Premium ultrabook",
                    Price = 1499m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 2
                },
                new
                {
                    Id = 6,
                    Name = "Asus ROG Strix",
                    Description = "Gaming laptop",
                    Price = 1799m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 2
                },

                new
                {
                    Id = 7,
                    Name = "iPad Pro M2",
                    Description = "Apple tablet",
                    Price = 1099m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 3
                },
                new
                {
                    Id = 8,
                    Name = "Samsung Galaxy Tab S9",
                    Description = "Android tablet",
                    Price = 899m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 3
                },

                new
                {
                    Id = 9,
                    Name = "AirPods Pro",
                    Description = "Wireless earbuds",
                    Price = 249m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 4
                },
                new
                {
                    Id = 10,
                    Name = "Apple Watch Series 9",
                    Description = "Smartwatch",
                    Price = 399m,
                    CreatedDate = created,
                    UpdatedDate = created,
                    CategoryId = 5
                }
            );

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
