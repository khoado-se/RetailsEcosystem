using RetailsEcosystem.Customer.Shared.DTOs.Cart;

namespace RetailsEcosystem.Customer.Web.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string accessToken);
        Task<CartDto> AddItemAsync(string accessToken, int productId, int quantity);
        Task<CartDto> UpdateItemAsync(string accessToken, int cartItemId, int quantity);
        Task<CartDto> RemoveItemAsync(string accessToken, int cartItemId);
        Task ClearCartAsync(string accessToken);
    }
}
