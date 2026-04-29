using RetailsEcosystem.Customer.Shared.DTOs.Cart;

namespace RetailsEcosystem.Customer.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> AddItemAsync(string userId, AddCartItemDto dto);
        Task<CartDto> UpdateItemAsync(string userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveItemAsync(string userId, int cartItemId);
        Task ClearCartAsync(string userId);
    }
}
