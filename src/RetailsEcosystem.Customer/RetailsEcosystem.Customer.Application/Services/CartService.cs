using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;

        public CartService(ICartRepository cartRepo, IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            return cart == null ? new CartDto() : MapToDto(cart);
        }

        public async Task<CartDto> AddItemAsync(string userId, AddCartItemDto dto)
        {
            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0.");

            var product = await _productRepo.GetProductByIdAsync(dto.ProductId)
                ?? throw new NotFoundException($"Product {dto.ProductId} not found.");

            if (product.StockQuantity < dto.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {product.StockQuantity}.");

            var cart = await _cartRepo.GetOrCreateAsync(userId);

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (existing != null)
            {
                var newQty = existing.Quantity + dto.Quantity;
                if (product.StockQuantity < newQty)
                    throw new InvalidOperationException(
                        $"Insufficient stock. Available: {product.StockQuantity}.");
                existing.Quantity = newQty;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }

            await _cartRepo.SaveAsync();
            return MapToDto(cart);
        }

        public async Task<CartDto> UpdateItemAsync(string userId, int cartItemId, UpdateCartItemDto dto)
        {
            if (dto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0.");

            var item = await _cartRepo.GetItemAsync(cartItemId)
                ?? throw new NotFoundException($"Cart item {cartItemId} not found.");

            if (item.Cart.UserId != userId)
                throw new UnauthorizedAccessException("Cart item does not belong to this user.");

            var product = await _productRepo.GetProductByIdAsync(item.ProductId)
                ?? throw new NotFoundException($"Product {item.ProductId} not found.");

            if (product.StockQuantity < dto.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {product.StockQuantity}.");

            item.Quantity = dto.Quantity;
            await _cartRepo.SaveAsync();
            return MapToDto(item.Cart);
        }

        public async Task<CartDto> RemoveItemAsync(string userId, int cartItemId)
        {
            var item = await _cartRepo.GetItemAsync(cartItemId)
                ?? throw new NotFoundException($"Cart item {cartItemId} not found.");

            if (item.Cart.UserId != userId)
                throw new UnauthorizedAccessException("Cart item does not belong to this user.");

            var cart = item.Cart;
            cart.Items.Remove(item);
            await _cartRepo.SaveAsync();
            return MapToDto(cart);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _cartRepo.GetByUserIdAsync(userId);
            if (cart == null) return;

            cart.Items.Clear();
            await _cartRepo.SaveAsync();
        }

        private static CartDto MapToDto(Cart cart) => new()
        {
            Id = cart.Id,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductImageUrl = i.Product?.Images.FirstOrDefault()?.Url,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}
