using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopDbContext _context;

        public HomeController(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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

            var viewModel = new HomeViewModel
            {
                // Get all categories
                Categories = await _context.Categories.ToListAsync(),

                // Get featured products with category and reviews
                FeaturedProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .OrderByDescending(p => p.Id)
                    .Take(12)
                    .ToListAsync(),

                // Get best sellers
                BestSellers = bestSellers
            };

            return View(viewModel);
        }
    }
}