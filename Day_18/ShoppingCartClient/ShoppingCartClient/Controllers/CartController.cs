using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Models;
using ShoppingCartClient.Services;
using System.Threading.Tasks;

namespace ShoppingCartClient.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IApiService _apiService;

        public CartController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await _apiService.GetCartAsync();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var dto = new AddToCartDto
            {
                ProductId = productId,
                Quantity = quantity
            };

            await _apiService.AddToCartAsync(dto);
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartAjax(int productId, int quantity = 1)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });

            var dto = new AddToCartDto
            {
                ProductId = productId,
                Quantity = quantity
            };

            var success = await _apiService.AddToCartAsync(dto);
            if (success)
            {
                // Fetch the updated cart state to send the badge count back
                var cart = await _apiService.GetCartAsync();
                int count = cart?.Items?.Sum(i => i.Quantity) ?? 0;
                return Json(new { success = true, cartCount = count });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var success = await _apiService.RemoveFromCartAsync(id);
            if (!success)
            {
                TempData["Error"] = "Could not remove the item from the cart.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false });

            var dto = new UpdateCartDto { Quantity = quantity };
            var result = await _apiService.UpdateCartItemAsync(id, dto);

            if (!result.Success)
            {
                var cart = await _apiService.GetCartAsync();
                int cartCount = cart?.Items?.Sum(i => i.Quantity) ?? 0;
                var updatedItem = cart?.Items?.FirstOrDefault(i => i.Id == id);
                decimal subtotal = updatedItem != null ? (updatedItem.UnitPrice * updatedItem.Quantity) : 0;
                decimal cartTotal = cart?.Items?.Sum(i => i.UnitPrice * i.Quantity) ?? 0;

                return Json(new
                {
                    success = true,
                    removed = (updatedItem == null),
                    subtotal = subtotal.ToString("0.00"),
                    cartTotal = cartTotal.ToString("0.00"),
                    cartCount = cartCount,
                    actualQuantity = updatedItem?.Quantity // 👈 ADD THIS LINE HERE
                });
            }

            return Json(new { success = false });
        }
    }
}
