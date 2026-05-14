using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories;

namespace RetailsEcosystem.Customer.Tests.Infrastructure.Repositories;

public class CategoryRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    private static Category Cat(int id, string name = "Category") =>
        new() { Id = id, Name = name, Description = "Desc" };

    [Fact]
    public async Task GetAllAsync_ReturnsPaged()
    {
        using var ctx = CreateContext();
        ctx.Categories.AddRange(Cat(1), Cat(2), Cat(3), Cat(4));
        await ctx.SaveChangesAsync();
        var repo = new CategoryRepository(ctx);

        var result = await repo.GetAllAsync(1, 2);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Found_ReturnsCategory()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(5, "Electronics"));
        await ctx.SaveChangesAsync();
        var repo = new CategoryRepository(ctx);

        var result = await repo.GetCategoryByIdAsync(5);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetCategoryByIdAsync_NotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new CategoryRepository(ctx);

        var result = await repo.GetCategoryByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_AddsCategory()
    {
        using var ctx = CreateContext();
        var repo = new CategoryRepository(ctx);

        await repo.CreateAsync(Cat(10, "New Cat"));
        await ctx.SaveChangesAsync();

        (await ctx.Categories.FindAsync(10)).Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_Found_UpdatesCategory()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1, "Old Name"));
        await ctx.SaveChangesAsync();
        var repo = new CategoryRepository(ctx);

        await repo.UpdateAsync(new Category { Id = 1, Name = "New Name", Description = "New Desc" });
        await ctx.SaveChangesAsync();

        (await ctx.Categories.FindAsync(1))!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        using var ctx = CreateContext();
        var repo = new CategoryRepository(ctx);

        var act = async () => await repo.UpdateAsync(Cat(999, "X"));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_Found_RemovesCategory()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        await ctx.SaveChangesAsync();
        var repo = new CategoryRepository(ctx);

        await repo.DeleteAsync(1);
        await ctx.SaveChangesAsync();

        (await ctx.Categories.FindAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        using var ctx = CreateContext();
        var repo = new CategoryRepository(ctx);

        var act = async () => await repo.DeleteAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetTotalCategoriesAsync_ReturnsCount()
    {
        using var ctx = CreateContext();
        ctx.Categories.AddRange(Cat(1), Cat(2), Cat(3));
        await ctx.SaveChangesAsync();
        var repo = new CategoryRepository(ctx);

        var count = await repo.GetTotalCategoriesAsync();

        count.Should().Be(3);
    }
}
