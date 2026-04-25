// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        public async Task<int> GetCartCountAsync(string userId)
        {
            return await CalculateCartCountAsync(userId);
        }
    }
}