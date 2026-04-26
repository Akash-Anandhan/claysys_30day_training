// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService : IWishlistService
    {
        private readonly ShopDbContext _context;
        public WishlistService(ShopDbContext context)
        {
            _context = context;
        }
    }
}