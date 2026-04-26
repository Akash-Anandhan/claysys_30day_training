// Services/OrderService.GetUserOrdersAsync.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class OrderService
    {
        public async Task<ServiceResponse> GetUserOrdersAsync(string userId, string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 5)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId);

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(o => o.Status == status);
            }

            // Apply date range filters
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= toDate.Value.Date.AddDays(1).AddSeconds(-1));
            }

            var totalOrders = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ViewModels.OrderPaginationViewModel
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalOrders = totalOrders,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate
            };

            return ServiceResponse.ShowView("Orders", viewModel);
        }
    }
}
