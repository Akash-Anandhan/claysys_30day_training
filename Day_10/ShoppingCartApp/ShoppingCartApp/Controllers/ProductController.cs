using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartApp.Controllers
{
    // Part 1 of ProductController
    // Contains the main product browsing actions
    public partial class ProductController : Controller
    {
        private readonly ShopDbContext _context;

        public ProductController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ToListAsync();

            return View(products);
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}