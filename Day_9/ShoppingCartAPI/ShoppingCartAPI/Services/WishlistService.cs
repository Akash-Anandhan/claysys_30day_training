using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class WishlistService : IWishlistService
    {
        private readonly ShopDbContext _context;

        public WishlistService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WishlistItemDto>> GetWishlistAsync(string userId)
        {
            var wishlist = await _context.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return wishlist.Select(w => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product?.Name,
                UnitPrice = w.Product?.Price ?? 0,
                ImageUrl = w.Product?.ImageUrl
            });
        }

        public async Task<string> AddToWishlistAsync(string userId, AddWishlistDto dto)
        {
            if (!await ProductExistsAsync(dto.ProductId))
                throw new KeyNotFoundException("Product not found.");

            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == dto.ProductId);

            if (existingItem != null)
                throw new ArgumentException("Product already in wishlist.");

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = dto.ProductId
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return "Product added to wishlist.";
        }

        public async Task<string> RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
                throw new KeyNotFoundException("Product not found in wishlist.");

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return "Product removed from wishlist.";
        }
    }
}
