// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService
    {
        public async Task<ServiceResponse> MoveToCartAsync(MoveToCartDto dto)
        {
            var wishlistItem = await _context.WishlistItems.Include(w => w.Product).FirstOrDefaultAsync(w => w.Id == dto.ItemId && w.UserId == dto.UserId);
            if (wishlistItem == null)
                return ServiceResponse.ShowView("NotFound", null, string.Empty, "Wishlist item not found.");
            // Add to cart — increment if already exists
            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == dto.UserId && c.ProductId == wishlistItem.ProductId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
            }
            else
            {
                _context.CartItems.Add(new CartItem { UserId = dto.UserId, ProductId = wishlistItem.ProductId, Quantity = 1, SellingPrice = wishlistItem.Product.SellingPrice });
            }

            // Remove from wishlist now that it's in the cart
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();
            return ServiceResponse.Redirect("Index", "Cart");
        }
    }
}