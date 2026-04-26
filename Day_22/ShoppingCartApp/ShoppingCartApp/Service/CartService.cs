// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService : ICartService
    {
        private readonly ShopDbContext _context;
        public CartService(ShopDbContext context)
        {
            _context = context;
        }
    }
}