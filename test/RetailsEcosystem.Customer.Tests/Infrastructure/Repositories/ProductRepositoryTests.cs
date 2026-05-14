using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories;

namespace RetailsEcosystem.Customer.Tests.Infrastructure.Repositories;

public class ProductRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    private static Category Cat(int id, string name = "Cat") =>
        new() { Id = id, Name = name, Description = "" };

    private static Product Prod(int id, int catId = 1, string name = "Product",
        decimal price = 100m, bool featured = false, int stock = 10,
        DateTime? createdDate = null) =>
        new()
        {
            Id            = id,
            Name          = name,
            Description   = "",
            Price         = price,
            StockQuantity = stock,
            IsFeatured    = featured,
            CategoryId    = catId,
            CreatedDate   = createdDate ?? DateTime.UtcNow,
            UpdatedDate   = DateTime.UtcNow
        };

    // ── GetAllProductAsync — filter branches ──────────────────────────────────

    [Fact]
    public async Task GetAll_NoCriteria_ReturnsAll()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1), Prod(2), Prod(3));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = await repo.GetAllProductAsync(1, 10);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAll_CategoryIdFilter_ReturnsMatchingOnly()
    {
        using var ctx = CreateContext();
        ctx.Categories.AddRange(Cat(1), Cat(2));
        ctx.Products.AddRange(Prod(1, catId: 1), Prod(2, catId: 2), Prod(3, catId: 1));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = await repo.GetAllProductAsync(1, 10, categoryId: 1);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_SearchFilter_ReturnsMatches()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, name: "Widget"), Prod(2, name: "Gadget"), Prod(3, name: "Wicker"));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = await repo.GetAllProductAsync(1, 10, search: "Wid");

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Widget");
    }

    [Fact]
    public async Task GetAll_IsFeaturedFilter_ReturnsOnlyFeatured()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, featured: true), Prod(2, featured: false), Prod(3, featured: true));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = await repo.GetAllProductAsync(1, 10, isFeatured: true);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_Pagination_ReturnsCorrectPage()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1), Prod(2), Prod(3), Prod(4), Prod(5));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var page2 = await repo.GetAllProductAsync(2, 2);

        page2.Should().HaveCount(2);
    }

    // ── GetAllProductAsync — sort branches (8-arm switch) ────────────────────

    [Fact]
    public async Task GetAll_SortByIdDesc_OrdersByIdDescending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1), Prod(2), Prod(3));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: "id", sortDesc: true)).ToList();

        result[0].Id.Should().BeGreaterThan(result[1].Id);
    }

    [Fact]
    public async Task GetAll_SortByIdAsc_OrdersByIdAscending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1), Prod(3), Prod(2));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: "id", sortDesc: false)).ToList();

        result[0].Id.Should().BeLessThan(result[1].Id);
    }

    [Fact]
    public async Task GetAll_SortByNameDesc_OrdersByNameDescending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, name: "A"), Prod(2, name: "C"), Prod(3, name: "B"));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: "name", sortDesc: true)).ToList();

        string.Compare(result[0].Name, result[1].Name, StringComparison.Ordinal).Should().BePositive();
    }

    [Fact]
    public async Task GetAll_SortByNameAsc_OrdersByNameAscending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, name: "C"), Prod(2, name: "A"), Prod(3, name: "B"));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: "name", sortDesc: false)).ToList();

        string.Compare(result[0].Name, result[1].Name, StringComparison.Ordinal).Should().BeNegative();
    }

    [Fact]
    public async Task GetAll_SortByPriceDesc_OrdersByPriceDescending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, price: 100m), Prod(2, price: 300m), Prod(3, price: 200m));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: "price", sortDesc: true)).ToList();

        result[0].Price.Should().BeGreaterThan(result[1].Price);
    }

    [Fact]
    public async Task GetAll_SortByPriceAsc_OrdersByPriceAscending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, price: 300m), Prod(2, price: 100m), Prod(3, price: 200m));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: "price", sortDesc: false)).ToList();

        result[0].Price.Should().BeLessThan(result[1].Price);
    }

    [Fact]
    public async Task GetAll_SortByCreatedDateAsc_OrdersByCreatedDateAscending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        var base_ = DateTime.UtcNow;
        ctx.Products.AddRange(
            Prod(1, createdDate: base_.AddDays(-2)),
            Prod(2, createdDate: base_.AddDays(-1)),
            Prod(3, createdDate: base_));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: "createddate", sortDesc: false)).ToList();

        result[0].CreatedDate.Should().BeBefore(result[1].CreatedDate);
    }

    [Fact]
    public async Task GetAll_DefaultSort_OrdersByCreatedDateDescending()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        var base_ = DateTime.UtcNow;
        ctx.Products.AddRange(
            Prod(1, createdDate: base_.AddDays(-2)),
            Prod(2, createdDate: base_),
            Prod(3, createdDate: base_.AddDays(-1)));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = (await repo.GetAllProductAsync(1, 10, sortBy: null)).ToList();

        result[0].CreatedDate.Should().BeOnOrAfter(result[1].CreatedDate);
    }

    // ── GetProductByIdAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetById_Found_ReturnsProduct()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.Add(Prod(1));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = await repo.GetProductByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new ProductRepository(ctx);

        var result = await repo.GetProductByIdAsync(999);

        result.Should().BeNull();
    }

    // ── GetProductCountAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetCount_IsFeatureTrue_CountsOnlyFeatured()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, featured: true), Prod(2, featured: false), Prod(3, featured: true));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var count = await repo.GetProductCountAsync(isFeature: true);

        count.Should().Be(2);
    }

    [Fact]
    public async Task GetCount_IsFeatureFalse_CountsAll()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1), Prod(2), Prod(3));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var count = await repo.GetProductCountAsync(isFeature: false);

        count.Should().Be(3);
    }

    [Fact]
    public async Task GetCount_WithSearch_CountsMatches()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(Prod(1, name: "Widget"), Prod(2, name: "Gadget"));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var count = await repo.GetProductCountAsync(search: "Wid");

        count.Should().Be(1);
    }

    // ── GetFeaturedProductsAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetFeatured_ReturnsOnlyFeaturedProducts()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.AddRange(
            Prod(1, featured: true),
            Prod(2, featured: false),
            Prod(3, featured: true));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var result = await repo.GetFeaturedProductsAsync(1, 10);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.IsFeatured);
    }

    // ── AddProductAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task Add_AddsProductToContext()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        await repo.AddProductAsync(Prod(99));
        await ctx.SaveChangesAsync();

        (await ctx.Products.FindAsync(99)).Should().NotBeNull();
    }

    // ── EditProductAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task Edit_Found_UpdatesProduct()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.Add(Prod(1, name: "Old Name"));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        var updated = Prod(1, name: "New Name");
        await repo.EditProductAsync(updated);
        await ctx.SaveChangesAsync();

        (await ctx.Products.FindAsync(1))!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task Edit_NotFound_ThrowsKeyNotFoundException()
    {
        using var ctx = CreateContext();
        var repo = new ProductRepository(ctx);

        var act = async () => await repo.EditProductAsync(Prod(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── RemoveProductAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task Remove_Found_RemovesProduct()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.Add(Prod(1));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        await repo.RemoveProductAsync(1);
        await ctx.SaveChangesAsync();

        (await ctx.Products.FindAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task Remove_NotFound_ThrowsKeyNotFoundException()
    {
        using var ctx = CreateContext();
        var repo = new ProductRepository(ctx);

        var act = async () => await repo.RemoveProductAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ── CheckExist ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CheckExist_ProductExists_ReturnsTrue()
    {
        using var ctx = CreateContext();
        ctx.Categories.Add(Cat(1));
        ctx.Products.Add(Prod(1));
        await ctx.SaveChangesAsync();
        var repo = new ProductRepository(ctx);

        (await repo.CheckExist(1)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckExist_ProductNotExists_ReturnsFalse()
    {
        using var ctx = CreateContext();
        var repo = new ProductRepository(ctx);

        (await repo.CheckExist(999)).Should().BeFalse();
    }
}
