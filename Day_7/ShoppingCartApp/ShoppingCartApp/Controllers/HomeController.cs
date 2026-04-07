using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Models;
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
            var products = await _context.Products
                .Include(p => p.Category)
                .Take(8)
                .ToListAsync();

            return View(products);
        }
    }
}