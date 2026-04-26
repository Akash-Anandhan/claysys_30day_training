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
        public async Task<ServiceResponse> DeleteProductAsync(int id)
        {
            var product = await _context.Products.Include(p => p.Reviews).Include(p => p.CartItems).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);
            _context.Reviews.RemoveRange(product.Reviews);
            _context.CartItems.RemoveRange(product.CartItems);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Products",
                TempData = new Dictionary<string, string>
                {
                    {
                        "Success",
                        $"Product '{product.Name}' deleted successfully!"}
                }
            };
        }
    }
}