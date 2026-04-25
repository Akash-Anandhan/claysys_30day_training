using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services.Interfaces;

namespace ShoppingCartAPI.Services
{
    public partial class WishlistService : IWishlistService
    {
        private readonly ShopDbContext _context;
        private readonly IUserContextService _userContextService;

        public WishlistService(ShopDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }
    }
}
