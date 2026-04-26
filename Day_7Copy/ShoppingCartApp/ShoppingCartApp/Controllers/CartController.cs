// Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Services;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers
{
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            return Execute(await _cartService.GetCartAsync(GetUserId()));
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var response = await _cartService.AddToCartAsync(new AddToCartDto
            {
                UserId = GetUserId(),
                ProductId = productId,
                Quantity = quantity,
                IsAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest"
            });

            // AJAX requests get JSON back — everything else goes through Execute()
            if (response.Succeeded && response.RedirectAction == null)
                return Json(new { success = true });

            return Execute(response);
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            return Execute(await _cartService.RemoveFromCartAsync(new RemoveFromCartDto
            {
                UserId = GetUserId(),
                ItemId = id
            }));
        }

        // POST: /Cart/UpdateQuantity (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var result = await _cartService.UpdateQuantityAsync(new UpdateQuantityDto
            {
                UserId = GetUserId(),
                ItemId = id,
                Quantity = quantity
            });

            return Json(result);
        }

        // GET: /Cart/GetCartCount (AJAX)
        public async Task<IActionResult> GetCartCount()
        {
            var count = await _cartService.GetCartCountAsync(GetUserId());
            return Json(new { count });
        }

        // ── Helpers ────────────────────────────────────────────────────────
        // Resolves userId from claims, or creates/retrieves a guest session ID.
        // This is an HTTP concern — it reads HttpContext — so it stays here.
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
                return userId;

            var guestId = HttpContext.Session.GetString("GuestId");
            if (string.IsNullOrEmpty(guestId))
            {
                guestId = "guest_" + Guid.NewGuid();
                HttpContext.Session.SetString("GuestId", guestId);
            }

            return guestId;
        }
    }
}