// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService
    {
        public async Task<int> GetWishlistCountAsync(string userId)
        {
            return await _context.WishlistItems.CountAsync(w => w.UserId == userId);
        }
    }
}