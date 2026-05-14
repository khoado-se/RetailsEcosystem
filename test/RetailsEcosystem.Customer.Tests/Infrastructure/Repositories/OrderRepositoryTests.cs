using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Tests.Infrastructure.Repositories;

public class OrderRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    private static ApplicationUser MakeUser(string id = "user1") =>
        new() { Id = id, UserName = $"{id}@test.com", NormalizedUserName = $"{id.ToUpper()}@TEST.COM",
                Email = $"{id}@test.com", NormalizedEmail = $"{id.ToUpper()}@TEST.COM",
                SecurityStamp = Guid.NewGuid().ToString() };

    private static Order MakeOrder(int id, string userId = "user1",
        OrderStatus status = OrderStatus.Pending,
        DateTime? createdDate = null, decimal total = 100m) =>
        new()
        {
            Id              = id,
            UserId          = userId,
            Status          = status,
            ShippingAddress = "123 St",
            TotalAmount     = total,
            CreatedDate     = createdDate ?? DateTime.UtcNow,
            UpdatedDate     = DateTime.UtcNow
        };

    // ── CreateOrderAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrderAsync_AddsOrderAndReturns()
    {
        using var ctx = CreateContext();
        var repo = new OrderRepository(ctx);
        var order = MakeOrder(1);

        var result = await repo.CreateOrderAsync(order);
        await ctx.SaveChangesAsync();

        result.Should().BeSameAs(order);
        ctx.Orders.Should().HaveCount(1);
    }

    // ── GetOrderByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrderByIdAsync_Found_ReturnsOrder()
    {
        using var ctx = CreateContext();
        ctx.Users.Add(MakeUser("user1"));
        ctx.Orders.Add(MakeOrder(1, "user1"));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var result = await repo.GetOrderByIdAsync(1);

        result.Should().NotBeNull();
        result!.UserId.Should().Be("user1");
    }

    [Fact]
    public async Task GetOrderByIdAsync_NotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new OrderRepository(ctx);

        var result = await repo.GetOrderByIdAsync(999);

        result.Should().BeNull();
    }

    // ── GetOrdersByUserIdAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetOrdersByUserIdAsync_ReturnsUserOrdersOnly()
    {
        using var ctx = CreateContext();
        ctx.Orders.AddRange(
            MakeOrder(1, "user1"),
            MakeOrder(2, "user2"),
            MakeOrder(3, "user1"));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var result = await repo.GetOrdersByUserIdAsync("user1", 1, 10);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(o => o.UserId == "user1");
    }

    // ── GetAllOrdersAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllOrdersAsync_NoFilter_ReturnsAll()
    {
        using var ctx = CreateContext();
        ctx.Users.Add(MakeUser("user1"));
        ctx.Orders.AddRange(
            MakeOrder(1, status: OrderStatus.Pending),
            MakeOrder(2, status: OrderStatus.Confirmed),
            MakeOrder(3, status: OrderStatus.Shipped));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var result = await repo.GetAllOrdersAsync(1, 10);

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllOrdersAsync_WithStatusFilter_ReturnsFiltered()
    {
        using var ctx = CreateContext();
        ctx.Users.Add(MakeUser("user1"));
        ctx.Orders.AddRange(
            MakeOrder(1, status: OrderStatus.Pending),
            MakeOrder(2, status: OrderStatus.Confirmed),
            MakeOrder(3, status: OrderStatus.Pending));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var result = await repo.GetAllOrdersAsync(1, 10, OrderStatus.Pending);

        result.Should().HaveCount(2);
    }

    // ── Count methods ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrderCountByUserIdAsync_ReturnsCount()
    {
        using var ctx = CreateContext();
        ctx.Orders.AddRange(MakeOrder(1, "user1"), MakeOrder(2, "user1"), MakeOrder(3, "user2"));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var count = await repo.GetOrderCountByUserIdAsync("user1");

        count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllOrderCountAsync_NoFilter_CountsAll()
    {
        using var ctx = CreateContext();
        ctx.Orders.AddRange(MakeOrder(1), MakeOrder(2), MakeOrder(3));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var count = await repo.GetAllOrderCountAsync();

        count.Should().Be(3);
    }

    [Fact]
    public async Task GetAllOrderCountAsync_WithStatusFilter_CountsMatching()
    {
        using var ctx = CreateContext();
        ctx.Orders.AddRange(
            MakeOrder(1, status: OrderStatus.Pending),
            MakeOrder(2, status: OrderStatus.Confirmed));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var count = await repo.GetAllOrderCountAsync(OrderStatus.Pending);

        count.Should().Be(1);
    }

    [Fact]
    public async Task GetPendingOrderCountAsync_ReturnsPendingOnly()
    {
        using var ctx = CreateContext();
        ctx.Orders.AddRange(
            MakeOrder(1, status: OrderStatus.Pending),
            MakeOrder(2, status: OrderStatus.Confirmed),
            MakeOrder(3, status: OrderStatus.Pending));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var count = await repo.GetPendingOrderCountAsync();

        count.Should().Be(2);
    }

    // ── Revenue / stats ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrdersTodayCountAsync_CountsOnlyTodayOrders()
    {
        using var ctx = CreateContext();
        ctx.Orders.AddRange(
            MakeOrder(1, createdDate: DateTime.UtcNow),
            MakeOrder(2, createdDate: DateTime.UtcNow.AddDays(-2)));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var count = await repo.GetOrdersTodayCountAsync();

        count.Should().Be(1);
    }

    [Fact]
    public async Task GetRevenueThisMonthAsync_SumsCurrentMonthOrders()
    {
        using var ctx = CreateContext();
        ctx.Orders.AddRange(
            MakeOrder(1, total: 200m, createdDate: DateTime.UtcNow),
            MakeOrder(2, total: 300m, createdDate: DateTime.UtcNow),
            MakeOrder(3, total: 500m, createdDate: DateTime.UtcNow.AddMonths(-2)));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var revenue = await repo.GetRevenueThisMonthAsync();

        revenue.Should().Be(500m);
    }

    [Fact]
    public async Task GetDailyRevenueAsync_ReturnsCorrectDaysCount()
    {
        using var ctx = CreateContext();
        ctx.Orders.Add(MakeOrder(1, total: 100m, createdDate: DateTime.UtcNow));
        await ctx.SaveChangesAsync();
        var repo = new OrderRepository(ctx);

        var result = (await repo.GetDailyRevenueAsync(7)).ToList();

        result.Should().HaveCount(7);
    }
}
