// Controllers/OrderController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Order;
using ShoppingCartApp.Services;
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
            return Execute(await _orderService.CheckoutAsync(new CheckoutDto
            {
                UserId = GetUserId()
            }));
        }

        // POST: /Order/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string shippingAddress, string paymentMethod)
        {
            return Execute(await _orderService.PlaceOrderAsync(new PlaceOrderDto
            {
                UserId = GetUserId(),
                ShippingAddress = shippingAddress,
                PaymentMethod = paymentMethod
            }));
        }

        // GET: /Order/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            return Execute(await _orderService.GetConfirmationAsync(new OrderConfirmationDto
            {
                UserId = GetUserId(),
                OrderId = id
            }));
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}