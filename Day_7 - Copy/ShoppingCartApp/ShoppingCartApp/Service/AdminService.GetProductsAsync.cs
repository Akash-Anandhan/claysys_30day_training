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
        // ── Products ───────────────────────────────────────────────────────────
        public async Task<ServiceResponse> GetProductsAsync()
        {
            var products = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).OrderByDescending(p => p.Id).ToListAsync();
            return ServiceResponse.ShowView("Products", products);
        }
    }
}