using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services.Interfaces;

namespace ShoppingCartAPI.Services
{
    public partial class OrdersService : IOrdersService
    {
        private readonly ShopDbContext _context;
        private readonly IUserContextService _userContextService;

        public OrdersService(ShopDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }
    }
}
