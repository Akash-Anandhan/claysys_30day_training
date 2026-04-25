// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService
    {
        public async Task<ServiceResponse> AddToWishlistAsync(AddToWishlistDto dto)
        {
            var alreadyExists = await _context.WishlistItems.AnyAsync(w => w.UserId == dto.UserId && w.ProductId == dto.ProductId);
            if (!alreadyExists)
            {
                _context.WishlistItems.Add(new WishlistItem { UserId = dto.UserId, ProductId = dto.ProductId, AddedOn = DateTime.Now });
                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Index", "Wishlist");
        }
    }
}