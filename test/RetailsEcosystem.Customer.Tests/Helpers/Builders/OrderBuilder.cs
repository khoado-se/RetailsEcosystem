using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared.Enums;

namespace RetailsEcosystem.Customer.Tests.Helpers.Builders;

public class OrderBuilder
{
    private int _id = 1;
    private string _userId = "user-123";
    private OrderStatus _status = OrderStatus.Pending;
    private decimal _totalAmount = 50.00m;
    private string _shippingAddress = "123 Test St";
    private List<OrderItem> _items = [];

    public OrderBuilder WithId(int id) { _id = id; return this; }
    public OrderBuilder WithUserId(string userId) { _userId = userId; return this; }
    public OrderBuilder WithStatus(OrderStatus status) { _status = status; return this; }
    public OrderBuilder WithTotalAmount(decimal total) { _totalAmount = total; return this; }

    public OrderBuilder WithItem(int productId = 1, string productName = "Test Product", decimal unitPrice = 10m, int qty = 2)
    {
        _items.Add(new OrderItem
        {
            Id = _items.Count + 1,
            OrderId = _id,
            ProductId = productId,
            ProductName = productName,
            UnitPrice = unitPrice,
            Quantity = qty
        });
        return this;
    }

    public Order Build() => new()
    {
        Id = _id,
        UserId = _userId,
        Status = _status,
        TotalAmount = _totalAmount,
        ShippingAddress = _shippingAddress,
        Items = _items,
        CreatedDate = DateTime.UtcNow,
        UpdatedDate = DateTime.UtcNow
    };
}
