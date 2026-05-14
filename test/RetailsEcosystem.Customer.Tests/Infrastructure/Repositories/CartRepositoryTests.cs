using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories;

namespace RetailsEcosystem.Customer.Tests.Infrastructure.Repositories;

public class CartRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task GetByUserIdAsync_Found_ReturnsCart()
    {
        using var ctx = CreateContext();
        ctx.Carts.Add(new Cart { Id = 1, UserId = "user1" });
        await ctx.SaveChangesAsync();
        var repo = new CartRepository(ctx);

        var result = await repo.GetByUserIdAsync("user1");

        result.Should().NotBeNull();
        result!.UserId.Should().Be("user1");
    }

    [Fact]
    public async Task GetByUserIdAsync_NotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new CartRepository(ctx);

        var result = await repo.GetByUserIdAsync("unknown");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrCreateAsync_CartExists_ReturnsExisting()
    {
        using var ctx = CreateContext();
        ctx.Carts.Add(new Cart { Id = 1, UserId = "user1" });
        await ctx.SaveChangesAsync();
        var repo = new CartRepository(ctx);

        var result = await repo.GetOrCreateAsync("user1");

        result.Id.Should().Be(1);
        ctx.Carts.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetOrCreateAsync_CartNotExists_CreatesNew()
    {
        using var ctx = CreateContext();
        var repo = new CartRepository(ctx);

        var result = await repo.GetOrCreateAsync("newuser");
        await ctx.SaveChangesAsync();

        result.UserId.Should().Be("newuser");
        ctx.Carts.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetItemAsync_Found_ReturnsItem()
    {
        using var ctx = CreateContext();
        var category = new Domain.Entities.Category { Id = 1, Name = "Cat", Description = "" };
        var product = new Domain.Entities.Product
        {
            Id = 1, Name = "P", Description = "", Price = 10, StockQuantity = 5,
            CategoryId = 1, CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow
        };
        var cart = new Cart { Id = 1, UserId = "user1" };
        var item = new CartItem { Id = 1, CartId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10m };
        ctx.Categories.Add(category);
        ctx.Products.Add(product);
        ctx.Carts.Add(cart);
        ctx.CartItems.Add(item);
        await ctx.SaveChangesAsync();
        var repo = new CartRepository(ctx);

        var result = await repo.GetItemAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetItemAsync_NotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new CartRepository(ctx);

        var result = await repo.GetItemAsync(999);

        result.Should().BeNull();
    }
}
