// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService : IWishlistService
    {
        private readonly ShopDbContext _context;

        // Public constants for controller messages
        public const string MessageAdded = "Added to wishlist";
        public const string MessageRemoved = "Removed from wishlist";
        public const string MessageNotFound = "Item not found";

        public WishlistService(ShopDbContext context)
        {
            _context = context;
        }
    }
}