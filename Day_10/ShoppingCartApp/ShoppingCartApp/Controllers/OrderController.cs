using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ShopDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Order/Checkout
        public async Task<IActionResult> Checkout()
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                    return RedirectToAction("Index", "Cart");

                var errors = new List<string>();

                foreach (var item in cartItems)
                {
                    if (item.Quantity > item.Product.Stock)
                    {
                        errors.Add($"{item.Product.Name} has only {item.Product.Stock} items available");
                    }
                }

                if (errors.Any())
                {
                    TempData["Error"] = string.Join(", ", errors);
                    return RedirectToAction("Index", "Cart");
                }

                return View(cartItems);
            }
            catch (Exception ex)
            {
                // Log error here
                return View("Error");
            }
        }
        // POST: /Order/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string shippingAddress)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            // ✅ STOCK CHECK
            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product.Stock)
                {
                    TempData["Error"] = $"Only {item.Product.Stock} items available for {item.Product.Name}";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // ✅ CREATE ORDER
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                ShippingAddress = shippingAddress,
                Status = "Pending",
                TotalAmount = cartItems.Sum(c => c.UnitPrice * c.Quantity),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice
                }).ToList()
            };

            // ✅ REDUCE STOCK (IMPORTANT)
            foreach (var item in cartItems)
            {
                item.Product.Stock -= item.Quantity;
            }

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        // GET: /Order/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}