using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class OrdersService : IOrdersService
    {
        private readonly ShopDbContext _context;

        public OrdersService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                PaymentType = o.PaymentType,
                PaymentId = o.PaymentId,
                Items = o.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });
        }

        public async Task<object> CheckoutAsync(string userId, CheckoutDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new UnauthorizedAccessException("User not found.");

            string finalAddress = ValidateAndSetShippingAddressAsync(user, dto.ShippingAddress);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new ArgumentException("Cart is empty.");

            var totalAmount = cartItems.Sum(c => c.Quantity * c.UnitPrice);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = "Pending",
                ShippingAddress = finalAddress,
                PaymentType = dto.PaymentType,
                PaymentId = dto.PaymentId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };

                if (item.Product != null)
                {
                    item.Product.Stock -= item.Quantity;
                }

                _context.OrderItems.Add(orderItem);
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return new { Message = "Checkout successful.", OrderId = order.Id };
        }
    }
}
