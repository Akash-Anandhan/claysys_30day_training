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
        public async Task<ServiceResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order != null)
            {
                order.Status = dto.Status;
                await _context.SaveChangesAsync();
            }

            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "OrderDetails",
                RouteValues = new
                {
                    id = dto.OrderId
                },
                TempData = new Dictionary<string, string>
                {
                    {
                        "Success",
                        $"Order #{dto.OrderId} status updated to {dto.Status}."}
                }
            };
        }
    }
}