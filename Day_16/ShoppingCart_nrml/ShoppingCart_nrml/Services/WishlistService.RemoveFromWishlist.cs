using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class WishlistService
    {
        public async Task<string> RemoveFromWishlistAsync(int productId)
        {
            var userId = _userContextService.GetUserId();
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

