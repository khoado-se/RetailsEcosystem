using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Tests.Helpers.Builders;

public class CartBuilder
{
    private int _id = 1;
    private string _userId = "user-123";
    private List<CartItem> _items = [];

    public CartBuilder WithId(int id) { _id = id; return this; }
    public CartBuilder WithUserId(string userId) { _userId = userId; return this; }

    public CartBuilder WithItem(int productId = 1, int quantity = 2, decimal unitPrice = 10m, int stock = 100)
    {
        var product = new ProductBuilder().WithId(productId).WithStock(stock).WithPrice(unitPrice).Build();
        _items.Add(new CartItem
        {
            Id = _items.Count + 1,
            CartId = _id,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Product = product
        });
        return this;
    }

    public CartBuilder WithItemEntity(CartItem item)
    {
        _items.Add(item);
        return this;
    }

    public Cart Build()
    {
        var cart = new Cart
        {
            Id = _id,
            UserId = _userId,
            CreatedDate = DateTime.UtcNow,
            Items = _items
        };
        foreach (var item in _items)
            item.Cart = cart;
        return cart;
    }
}
