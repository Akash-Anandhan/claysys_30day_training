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
        public async Task<AdminOrderDetailDto> GetOrderDetailAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                return null;
            var user = await _userManager.FindByIdAsync(order.UserId);
            return new AdminOrderDetailDto
            {
                Order = order,
                UserEmail = user?.Email
            };
        }
    }
}