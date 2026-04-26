// Services/OrderService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Order;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class OrderService
    {
        public async Task<ServiceResponse> PlaceOrderAsync(PlaceOrderDto dto)
        {
            var cartItems = await _context.CartItems.Include(c => c.Product).Where(c => c.UserId == dto.UserId).ToListAsync();
            if (!cartItems.Any())
                return ServiceResponse.Redirect("Index", "Cart");
            // Stock check — fail fast on first violation
            var stockViolation = cartItems.FirstOrDefault(c => c.Quantity > c.Product.Stock);
            if (stockViolation != null)
                return new ServiceResponse
                {
                    Succeeded = true,
                    RedirectAction = "Index",
                    RedirectController = "Cart",
                    TempData = new Dictionary<string, string>
                    {
                        {
                            "Error",
                            $"Only {stockViolation.Product.Stock} items available " + $"for {stockViolation.Product.Name}"}
                    }
                };
            // Create order
            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.Now,
                ShippingAddress = dto.ShippingAddress,
                PhoneNumber = dto.PhoneNumber,
                PaymentMethod = string.IsNullOrEmpty(dto.PaymentMethod) ? "Credit Card" : dto.PaymentMethod,
                Status = "Pending",
                TotalAmount = cartItems.Sum(c => c.SellingPrice * c.Quantity),
                OrderItems = cartItems.Select(c => new OrderItem { ProductId = c.ProductId, Quantity = c.Quantity, SellingPrice = c.SellingPrice, BasePrice = c.BasePrice }).ToList()
            };
            // Reduce stock
            foreach (var item in cartItems)
                item.Product.Stock -= item.Quantity;
            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return ServiceResponse.Redirect("Confirmation", "Order", new { id = order.Id });
        }
    }
}