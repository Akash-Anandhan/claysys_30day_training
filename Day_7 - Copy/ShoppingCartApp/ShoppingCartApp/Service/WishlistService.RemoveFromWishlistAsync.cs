// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService
    {
        public async Task<ServiceResponse> RemoveFromWishlistAsync(RemoveFromWishlistDto dto)
        {
            var item = await _context.WishlistItems.FirstOrDefaultAsync(w => w.Id == dto.ItemId && w.UserId == dto.UserId);
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Index", "Wishlist");
        }
    }
}