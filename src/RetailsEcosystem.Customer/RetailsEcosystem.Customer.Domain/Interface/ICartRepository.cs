using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Domain.Interface
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(string userId);
        Task<Cart> GetOrCreateAsync(string userId);
        Task<CartItem?> GetItemAsync(int cartItemId);
        Task SaveAsync();
    }
}
