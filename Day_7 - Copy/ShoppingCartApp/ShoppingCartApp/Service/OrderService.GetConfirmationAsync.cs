// Services/OrderService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Order;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class OrderService
    {
        public async Task<ServiceResponse> GetConfirmationAsync(OrderConfirmationDto dto)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.UserId == dto.UserId);
            if (order == null)
                return ServiceResponse.ShowView("NotFound", null, string.Empty, "Order not found.");
            var confirmationPage = new OrderConfirmationPageDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Items = order.OrderItems.Select(oi => new OrderItemDto { ProductName = oi.Product.Name, ImageUrl = oi.Product.ImageUrl, Quantity = oi.Quantity, UnitPrice = oi.SellingPrice, Subtotal = oi.SellingPrice * oi.Quantity }).ToList()
            };
            return ServiceResponse.ShowView("Confirmation", confirmationPage);
        }
    }
}