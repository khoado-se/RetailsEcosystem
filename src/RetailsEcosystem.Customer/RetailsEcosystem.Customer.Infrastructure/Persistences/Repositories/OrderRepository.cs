using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) => _context = context;

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId, int pageNumber, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedDate)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, OrderStatus? status = null)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.User)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<int> GetOrderCountByUserIdAsync(string userId) =>
            _context.Orders.CountAsync(o => o.UserId == userId);

        public Task<int> GetAllOrderCountAsync(OrderStatus? status = null) =>
            status.HasValue
                ? _context.Orders.CountAsync(o => o.Status == status.Value)
                : _context.Orders.CountAsync();

        public Task<int> GetOrdersTodayCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return _context.Orders.CountAsync(o => o.CreatedDate >= today);
        }

        public async Task<decimal> GetRevenueThisMonthAsync()
        {
            var firstOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return await _context.Orders
                .Where(o => o.CreatedDate >= firstOfMonth)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
        }

        public Task<int> GetPendingOrderCountAsync() =>
            _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);

        public async Task<IEnumerable<DailyRevenueDto>> GetDailyRevenueAsync(int days)
        {
            var cutoff = DateTime.UtcNow.Date.AddDays(-(days - 1));

            var rawData = await _context.Orders
                .Where(o => o.CreatedDate >= cutoff)
                .GroupBy(o => o.CreatedDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => (decimal?)o.TotalAmount) ?? 0m })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Enumerable.Range(0, days)
                .Select(i => cutoff.AddDays(i))
                .Select(date => new DailyRevenueDto
                {
                    Date = date.ToString("MMM d"),
                    Revenue = rawData.FirstOrDefault(r => r.Date == date)?.Revenue ?? 0m
                });
        }

        public Task SaveAsync() => _context.SaveChangesAsync();
    }
}
