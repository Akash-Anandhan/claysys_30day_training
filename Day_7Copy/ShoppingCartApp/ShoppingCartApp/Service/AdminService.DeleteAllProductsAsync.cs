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
        public async Task<ServiceResponse> DeleteAllProductsAsync()
        {
            _context.CartItems.RemoveRange(await _context.CartItems.ToListAsync());
            _context.Reviews.RemoveRange(await _context.Reviews.ToListAsync());
            _context.OrderItems.RemoveRange(await _context.OrderItems.ToListAsync());
            _context.Products.RemoveRange(await _context.Products.ToListAsync());
            await _context.SaveChangesAsync();
            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Products",
                TempData = new Dictionary<string, string>
                {
                    {
                        "Success",
                        "All products deleted successfully!"
                    }
                }
            };
        }
    }
}