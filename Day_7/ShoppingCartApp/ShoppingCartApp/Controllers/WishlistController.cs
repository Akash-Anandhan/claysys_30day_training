using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ShopDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Wishlist
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);

            var wishlistItems = await _context.WishlistItems
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedOn)
                .ToListAsync();

            return View(wishlistItems);
        }

        // POST: /Wishlist/Add
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            string userId = _userManager.GetUserId(User);

            // Check if already in wishlist
            var existing = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existing == null)
            {
                var wishlistItem = new WishlistItem
                {
                    UserId = userId,
                    ProductId = productId,
                    AddedOn = DateTime.Now
                };
                _context.WishlistItems.Add(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST: /Wishlist/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // POST: /Wishlist/MoveToCart
        [HttpPost]
        public async Task<IActionResult> MoveToCart(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var wishlistItem = await _context.WishlistItems
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (wishlistItem == null)
                return NotFound();

            // Check if already in cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == wishlistItem.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = wishlistItem.ProductId,
                    Quantity = 1,
                    UnitPrice = wishlistItem.Product.Price
                };
                _context.CartItems.Add(cartItem);
            }

            // Remove from wishlist
            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Cart");
        }

        // GET: /Wishlist/GetWishlistCount
        public async Task<IActionResult> GetWishlistCount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var count = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .CountAsync();

            return Json(new { count });
        }
    }
}