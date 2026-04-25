// Services/AdminService.GetOrdersAsync.cs
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoppingCartApp.DTOs.Admin;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Globalization;
using System.Linq;

namespace ShoppingCartApp.Services
{
    public partial class AdminService
    {
        // ── Orders ─────────────────────────────────────────────────────────────
        public async Task<AdminOrdersDto> GetOrdersAsync(
            string searchQuery = null,
            string statusFilter = null,
            string sortBy = null,
            int page = 1,
            int pageSize = 50)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            // Search by order ID or user email/name
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchLower = searchQuery.ToLower();
                var matchingUserIds = await _userManager.Users
                    .Where(u => u.Email.ToLower().Contains(searchLower) || 
                               (u.FullName != null && u.FullName.ToLower().Contains(searchLower)))
                    .Select(u => u.Id)
                    .ToListAsync();
                
                query = query.Where(o => 
                    o.Id.ToString().Contains(searchLower) || 
                    matchingUserIds.Contains(o.UserId));
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "all")
            {
                query = query.Where(o => o.Status == statusFilter);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy switch
            {
                "oldest" => query.OrderBy(o => o.OrderDate),
                "amount_high" => query.OrderByDescending(o => o.TotalAmount),
                "amount_low" => query.OrderBy(o => o.TotalAmount),
                _ => query.OrderByDescending(o => o.OrderDate) // newest (default)
            };

            // Apply pagination
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get user emails
            var userIds = orders.Select(o => o.UserId).Distinct().ToList();
            var userEmails = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            return new AdminOrdersDto
            {
                Orders = orders,
                UserEmails = userEmails,
                TotalCount = totalCount,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        // ── Export Orders ──────────────────────────────────────────────────────
        public async Task<(byte[] bytes, string contentType, string fileName)> ExportOrdersExcelAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Orders");

            // Headers
            ws.Cells[1, 1].Value = "Order ID";
            ws.Cells[1, 2].Value = "User ID";
            ws.Cells[1, 3].Value = "Order Date";
            ws.Cells[1, 4].Value = "Total Amount";
            ws.Cells[1, 5].Value = "Status";
            ws.Cells[1, 6].Value = "Items Count";

            int row = 2;
            foreach (var order in orders)
            {
                ws.Cells[row, 1].Value = order.Id;
                ws.Cells[row, 2].Value = order.UserId;
                ws.Cells[row, 3].Value = order.OrderDate.ToString("yyyy-MM-dd HH:mm");
                ws.Cells[row, 4].Value = order.TotalAmount;
                ws.Cells[row, 5].Value = order.Status;
                ws.Cells[row, 6].Value = order.OrderItems?.Count ?? 0;
                row++;
            }

            return (package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
        }

        public async Task<(byte[] bytes, string contentType, string fileName)> ExportOrdersCsvAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new { 
                    o.Id, 
                    o.UserId, 
                    OrderDate = o.OrderDate.ToString("yyyy-MM-dd HH:mm"),
                    o.TotalAmount, 
                    o.Status,
                    ItemsCount = o.OrderItems != null ? o.OrderItems.Count : 0
                })
                .ToListAsync();

            using var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream, leaveOpen: true);
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(orders);
            await writer.FlushAsync();
            return (stream.ToArray(), "text/csv", "Orders.csv");
        }
    }
}