// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        public async Task MergeGuestCartAsync(string guestId, string userId)
        {
            var guestItems = await _context.CartItems.Where(c => c.UserId == guestId).ToListAsync();
            foreach (var guestItem in guestItems)
            {
                var existingItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == guestItem.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += guestItem.Quantity;
                    _context.CartItems.Remove(guestItem);
                }
                else
                {
                    guestItem.UserId = userId;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}