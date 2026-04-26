// Services/OrderService.CancelOrderAsync.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class OrderService
    {
        public async Task<ServiceResponse> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return new ServiceResponse { Succeeded = false, RedirectAction = "Orders", RedirectController = "Account" };
            }

            if (order.Status != "Pending")
            {
                return new ServiceResponse 
                { 
                    Succeeded = false, 
                    RedirectAction = "Orders", 
                    RedirectController = "Account",
                    TempData = new Dictionary<string, string> { { "Error", "Only pending orders can be cancelled." } }
                };
            }

            // Mark as cancelled
            order.Status = "Cancelled";

            // Restock items
            foreach (var item in order.OrderItems)
            {
                item.Product.Stock += item.Quantity;
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Orders",
                RedirectController = "Account",
                TempData = new Dictionary<string, string> { { "Success", "Order cancelled successfully." } }
            };
        }
    }
}
