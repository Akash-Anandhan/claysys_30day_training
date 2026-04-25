using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class WishlistService
    {
        public async Task<string> AddToWishlistAsync(AddWishlistDto dto)
        {
            var userId = _userContextService.GetUserId();
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
    }
}
