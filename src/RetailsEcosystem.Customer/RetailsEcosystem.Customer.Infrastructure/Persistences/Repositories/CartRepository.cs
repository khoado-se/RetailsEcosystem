using Microsoft.EntityFrameworkCore;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;

namespace RetailsEcosystem.Customer.Infrastructure.Persistences.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> GetOrCreateAsync(string userId)
        {
            var cart = await GetByUserIdAsync(userId);
            if (cart != null) return cart;

            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            return cart;
        }

        public async Task<CartItem?> GetItemAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(i => i.Cart)
                    .ThenInclude(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Images)
                .Include(i => i.Product)
                    .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(i => i.Id == cartItemId);
        }
    }
}
