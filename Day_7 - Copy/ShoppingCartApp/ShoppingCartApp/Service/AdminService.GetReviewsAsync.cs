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
        // ── Reviews ────────────────────────────────────────────────────────────
        public async Task<ServiceResponse> GetReviewsAsync()
        {
            var reviews = await _context.Reviews.Include(r => r.Product).Include(r => r.User).OrderByDescending(r => r.CreatedAt).ToListAsync();
            return ServiceResponse.ShowView("Reviews", reviews);
        }
    }
}