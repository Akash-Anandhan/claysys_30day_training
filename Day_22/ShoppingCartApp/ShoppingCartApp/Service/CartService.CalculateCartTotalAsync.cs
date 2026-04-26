// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        // ── Private helpers ────────────────────────────────────────────────
        private async Task<string> CalculateCartTotalAsync(string userId)
        {
            var total = await _context.CartItems.Where(c => c.UserId == userId).SumAsync(c => c.SellingPrice * c.Quantity);
            return total.ToString("0.00");
        }
    }
}