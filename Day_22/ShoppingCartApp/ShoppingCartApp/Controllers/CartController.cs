// Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Services;
using ShoppingCartApp.DTOs.Cart;
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
            var response = await _cartService.GetCartAsync(GetUserId());
            return View(response.ViewModel);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var dto = new AddToCartDto
            {
                UserId = GetUserId(),
                ProductId = productId,
                Quantity = quantity,
                IsAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest"
            };

            var response = await _cartService.AddToCartAsync(dto);

            if (response.Succeeded && response.RedirectAction == null)
                return Json(new { success = true });

            return await ExecuteServiceResponse(response);
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var dto = new RemoveFromCartDto
            {
                UserId = GetUserId(),
                ItemId = id
            };
            return await ExecuteServiceResponse(await _cartService.RemoveFromCartAsync(dto));
        }

        // POST: /Cart/UpdateQuantity (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var dto = new UpdateQuantityDto
            {
                UserId = GetUserId(),
                ItemId = id,
                Quantity = quantity
            };
            return Json(await _cartService.UpdateQuantityAsync(dto));
        }

        // GET: /Cart/GetCartCount (AJAX)
        public async Task<IActionResult> GetCartCount()
        {
            var count = await _cartService.GetCartCountAsync(GetUserId());
            return Json(new { count });
        }

        // Helper method to handle ServiceResponse
        private async Task<IActionResult> ExecuteServiceResponse(ServiceResponse response)
        {
            foreach (var (key, value) in response.TempData)
                TempData[key] = value;

            foreach (var (key, message) in response.ModelErrors)
                ModelState.AddModelError(key, message);

            if (response.Succeeded && response.RedirectAction != null)
                return response.RedirectController != null
                    ? RedirectToAction(response.RedirectAction, response.RedirectController, response.RouteValues)
                    : RedirectToAction(response.RedirectAction, response.RouteValues);

            return View(response.ViewName, response.ViewModel);
        }

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