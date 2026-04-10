using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ShopDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Gets userId if logged in, otherwise returns session guest ID
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
                return userId;

            // Guest user — create a session ID if not exists
            var guestId = HttpContext.Session.GetString("GuestId");
            if (string.IsNullOrEmpty(guestId))
            {
                guestId = "guest_" + Guid.NewGuid().ToString();
                HttpContext.Session.SetString("GuestId", guestId);
            }

            return guestId;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            string userId = GetUserId();

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
            string userId = GetUserId();

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

            // Return JSON if AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            string userId = GetUserId();

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            string userId = GetUserId();

            var item = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (item == null)
                return Json(new { success = false, message = "Item not found" });

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    removed = true,
                    cartTotal = await GetCartTotalAsync(userId)
                });
            }

            item.Quantity = quantity;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                removed = false,
                subtotal = (item.UnitPrice * item.Quantity).ToString("0.00"),
                cartTotal = await GetCartTotalAsync(userId),
                cartCount = await GetCartCountAsync(userId)
            });
        }

        // GET: /Cart/GetCartCount
        public async Task<IActionResult> GetCartCount()
        {
            string userId = GetUserId();
            var count = await GetCartCountAsync(userId);
            return Json(new { count });
        }

        // Helper — get total price
        private async Task<string> GetCartTotalAsync(string userId)
        {
            var total = await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.UnitPrice * c.Quantity);

            return total.ToString("0.00");
        }

        // Helper — get total item count
        private async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }

        // Merge guest cart into user cart after login
        public async Task MergeGuestCartAsync(string guestId, string userId)
        {
            var guestItems = await _context.CartItems
                .Where(c => c.UserId == guestId)
                .ToListAsync();

            foreach (var guestItem in guestItems)
            {
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId
                        && c.ProductId == guestItem.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += guestItem.Quantity;
                    _context.CartItems.Remove(guestItem);
                }
                else
                {
                    guestItem.UserId = userId;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}