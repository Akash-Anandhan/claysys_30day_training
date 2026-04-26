// Services/OrderService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShoppingCartApp.DTOs.Order;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class OrderService : IOrderService
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public OrderService(ShopDbContext context, UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _cache = cache;
        }

        // Get order by ID with permission check (for tracking)
        public async Task<Order?> GetOrderByIdAsync(int orderId, string userId)
        {
            var cacheKey = $"order_{orderId}_{userId}";
            
            if (!_cache.TryGetValue(cacheKey, out var order))
            {
                order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order != null)
                {
                    _cache.Set(cacheKey, order, CacheDuration);
                }
            }

            return (Order?)order;
        }
    }
}