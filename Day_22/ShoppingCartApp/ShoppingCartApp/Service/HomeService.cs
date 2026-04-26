// Services/HomeService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Services
{
    public class HomeService : IHomeService
    {
        private readonly ShopDbContext _context;

        public HomeService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<HomeViewModel> GetHomeViewModelAsync()
        {
            // Get top selling products for best sellers via OrderItems join
            var topProductIds = await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Take(4)
                .Select(g => g.Key)
                .ToListAsync();

            var bestSellers = topProductIds.Any()
                ? await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .Where(p => topProductIds.Contains(p.Id))
                    .ToListAsync()
                : new List<Product>();

            return new HomeViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                FeaturedProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .OrderByDescending(p => p.Id)
                    .Take(12)
                    .ToListAsync(),
                BestSellers = bestSellers
            };
        }
    }
}