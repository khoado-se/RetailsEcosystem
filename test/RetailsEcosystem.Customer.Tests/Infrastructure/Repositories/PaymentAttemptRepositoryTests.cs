using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Infrastructure.Persistences;
using RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Tests.Infrastructure.Repositories;

public class PaymentAttemptRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task AddAsync_AddsAttemptToContext()
    {
        using var ctx = CreateContext();
        ctx.Orders.Add(new Order
        {
            Id = 1, UserId = "u1", ShippingAddress = "st",
            CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();
        var repo = new PaymentAttemptRepository(ctx);
        var attempt = new PaymentAttempt
        {
            TxnRef    = "1-20260101",
            OrderId   = 1,
            Status    = PaymentAttemptStatus.Initiated,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IpAddress = "127.0.0.1"
        };

        await repo.AddAsync(attempt);
        await ctx.SaveChangesAsync();

        ctx.PaymentAttempts.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByTxnRefAsync_Found_ReturnsAttempt()
    {
        using var ctx = CreateContext();
        ctx.Orders.Add(new Order
        {
            Id = 1, UserId = "u1", ShippingAddress = "st",
            CreatedDate = DateTime.UtcNow, UpdatedDate = DateTime.UtcNow
        });
        ctx.PaymentAttempts.Add(new PaymentAttempt
        {
            TxnRef    = "1-20260101",
            OrderId   = 1,
            Status    = PaymentAttemptStatus.Initiated,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IpAddress = "127.0.0.1"
        });
        await ctx.SaveChangesAsync();
        var repo = new PaymentAttemptRepository(ctx);

        var result = await repo.GetByTxnRefAsync("1-20260101");

        result.Should().NotBeNull();
        result!.TxnRef.Should().Be("1-20260101");
    }

    [Fact]
    public async Task GetByTxnRefAsync_NotFound_ReturnsNull()
    {
        using var ctx = CreateContext();
        var repo = new PaymentAttemptRepository(ctx);

        var result = await repo.GetByTxnRefAsync("nonexistent");

        result.Should().BeNull();
    }
}
