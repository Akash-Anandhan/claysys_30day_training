using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class CartService : ICartService
    {
        private readonly ShopDbContext _context;

        public CartService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItemResponseDto>> GetCartAsync(string userId)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return cartItems.Select(c => new CartItemResponseDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product?.Name,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice,
                TotalPrice = c.Quantity * c.UnitPrice
            });
        }

        public async Task<string> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (product.Stock < dto.Quantity)
                throw new ArgumentException("Not enough stock.");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return "Item added to cart successfully.";
        }

        public async Task<string> RemoveFromCartAsync(string userId, int id)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem == null)
                throw new KeyNotFoundException("Item not found in cart.");

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return "Item removed from cart.";
        }

        public async Task<object> UpdateCartItemAsync(string userId, int id, UpdateCartDto dto)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem == null)
                throw new KeyNotFoundException("Item not found in cart.");

            var product = await GetProductInfoAsync(cartItem.ProductId);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            if (product.Stock < dto.Quantity)
                throw new ArgumentException("Not enough stock.");

            cartItem.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return new
            {
                Message = "Cart item updated successfully.",
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity
            };
        }
    }
}
