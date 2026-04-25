using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Models;
using ShoppingCartClient.Services;
using System.Threading.Tasks;

namespace ShoppingCartClient.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IApiService _apiService;

        public OrderController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Checkout()
        {
            return View(new CheckoutDto());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto model)
        {
            if (ModelState.IsValid)
            {
                var orderId = await _apiService.CheckoutAsync(model);
                if (orderId > 0)
                {
                    return RedirectToAction("Confirmation", new { id = orderId });
                }
                ModelState.AddModelError("", "Checkout failed. Please try again or verify stock.");
            }
            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var orders = await _apiService.GetOrdersAsync();
            var order = orders?.FirstOrDefault(o => o.Id == id);
            
            if (order == null)
            {
                // Fallback to empty confirmation if not found
                return View();
            }

            return View(order);
        }
    }
}
