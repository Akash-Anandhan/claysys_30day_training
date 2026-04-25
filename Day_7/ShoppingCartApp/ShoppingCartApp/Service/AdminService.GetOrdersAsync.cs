// Services/AdminService.cs
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoppingCartApp.DTOs.Admin;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Globalization;

namespace ShoppingCartApp.Services
{
    public partial class AdminService
    {
        // ── Orders ─────────────────────────────────────────────────────────────
        public async Task<AdminOrdersDto> GetOrdersAsync()
        {
            var orders = await _context.Orders.Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate).ToListAsync();
            var userIds = orders.Select(o => o.UserId).Distinct().ToList();
            var userEmails = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.Email);
            return new AdminOrdersDto
            {
                Orders = orders,
                UserEmails = userEmails
            };
        }
    }
}