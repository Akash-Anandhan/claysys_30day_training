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
        public async Task<(byte[] bytes, string contentType, string fileName)> ExportCsvAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.CreatedAt <= endOfDay);
            }

            var products = await query.Select(p => new { 
                p.Id, 
                p.Name, 
                p.Description, 
                p.SellingPrice, 
                p.Stock, 
                Category = p.Category != null ? p.Category.Name : "" 
            }).ToListAsync();

            using var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream, leaveOpen: true);
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(products);
            await writer.FlushAsync();
            return (stream.ToArray(), "text/csv", "Products.csv");
        }
    }
}