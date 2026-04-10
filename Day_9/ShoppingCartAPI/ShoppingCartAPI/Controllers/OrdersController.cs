using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public OrdersController(ShopDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            string userId = GetUserId();
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var dtos = orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                Items = o.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });

            return Ok(dtos);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            string userId = GetUserId();
            
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest(new { Message = "Cart is empty." });

            var totalAmount = cartItems.Sum(c => c.Quantity * c.UnitPrice);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = "Pending",
                ShippingAddress = dto.ShippingAddress
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // To get Order ID

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                
                // Deduct stock
                if (item.Product != null)
                {
                    item.Product.Stock -= item.Quantity;
                }

                _context.OrderItems.Add(orderItem);
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Checkout successful.", OrderId = order.Id });
        }
    }
}
