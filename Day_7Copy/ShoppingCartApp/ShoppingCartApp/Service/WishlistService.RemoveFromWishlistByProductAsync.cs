// Service/WishlistService.RemoveFromWishlistByProductAsync.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService
    {
        public async Task<ServiceResponse> RemoveFromWishlistByProductAsync(string userId, int productId)
        {
            var item = await _context.WishlistItems.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
                return new ServiceResponse { Succeeded = true };
            }
            return new ServiceResponse { Succeeded = false };
        }
    }
}