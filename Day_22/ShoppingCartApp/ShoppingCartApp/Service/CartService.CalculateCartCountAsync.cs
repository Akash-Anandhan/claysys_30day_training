// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        private async Task<int> CalculateCartCountAsync(string userId)
        {
            return await _context.CartItems.Where(c => c.UserId == userId).SumAsync(c => c.Quantity);
        }
    }
}