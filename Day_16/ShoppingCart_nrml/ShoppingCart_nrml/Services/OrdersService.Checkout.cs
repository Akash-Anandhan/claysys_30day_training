using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class OrdersService
    {
        public async Task<object> CheckoutAsync(CheckoutDto dto)
        {
            var userId = _userContextService.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
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
                if (item.Product == null)
                    throw new Exception("Product not found.");

                if (item.Product.Stock < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Product '{item.Product.Name}' is out of stock. Available: {item.Product.Stock}, Requested: {item.Quantity}"
                    );
                }
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return new { Message = "Checkout successful.", OrderId = order.Id };
        }
    }
}



