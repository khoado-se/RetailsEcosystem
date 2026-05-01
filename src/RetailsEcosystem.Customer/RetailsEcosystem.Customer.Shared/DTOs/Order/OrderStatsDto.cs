namespace RetailsEcosystem.Customer.Shared.DTOs.Order
{
    public class OrderStatsDto
    {
        public int OrdersToday { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public int PendingOrders { get; set; }
        public IEnumerable<DailyRevenueDto> DailyRevenue { get; set; } = [];
    }
}
