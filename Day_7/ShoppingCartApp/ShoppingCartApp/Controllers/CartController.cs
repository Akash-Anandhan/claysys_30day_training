using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ShopDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId
                    && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (item != null)
            {
                if (quantity <= 0)
                    _context.CartItems.Remove(item);
                else
                    item.Quantity = quantity;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // GET: /Cart/GetCartCount
        public async Task<IActionResult> GetCartCount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Json(new { count = 0 });

            var count = await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);

            return Json(new { count });
        }
    }
}