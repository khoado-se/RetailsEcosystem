namespace RetailsEcosystem.Customer.Shared.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public IList<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal Total => Items.Sum(i => i.LineTotal);
        public int ItemCount => Items.Sum(i => i.Quantity);
    }
}
