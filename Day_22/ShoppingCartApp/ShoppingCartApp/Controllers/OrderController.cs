// Controllers/OrderController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Services;
using ShoppingCartApp.DTOs.Order;
using System.Security.Claims;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /Order/Checkout
        public async Task<IActionResult> Checkout()
        {
            var dto = new CheckoutDto { UserId = GetUserId() };
            return await ExecuteServiceResponse(await _orderService.CheckoutAsync(dto));
        }

        // POST: /Order/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(
            string fullName,
            string streetAddress,
            string city,
            string state,
            string postalCode,
            string country,
            string phone,
            string paymentMethod)
        {
            var shippingAddress = $"{streetAddress}, {city}, {state} {postalCode}, {country}";
            var dto = new PlaceOrderDto
            {
                UserId = GetUserId(),
                ShippingAddress = shippingAddress,
                PhoneNumber = phone,
                PaymentMethod = paymentMethod
            };
            return await ExecuteServiceResponse(await _orderService.PlaceOrderAsync(dto));
        }

        // GET: /Order/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var dto = new OrderConfirmationDto
            {
                UserId = GetUserId(),
                OrderId = id
            };
            return await ExecuteServiceResponse(await _orderService.GetConfirmationAsync(dto));
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

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}