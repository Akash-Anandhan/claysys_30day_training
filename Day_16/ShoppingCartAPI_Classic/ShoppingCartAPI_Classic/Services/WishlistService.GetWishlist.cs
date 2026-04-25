using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class WishlistService
    {
        public async Task<IEnumerable<WishlistItemDto>> GetWishlistAsync()
        {
            var userId = _userContextService.GetUserId();
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
    }
}
