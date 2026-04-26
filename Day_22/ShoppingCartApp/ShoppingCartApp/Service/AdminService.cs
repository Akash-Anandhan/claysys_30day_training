// Services/AdminService.cs
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoppingCartApp.DTOs.Admin;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Globalization;

namespace ShoppingCartApp.Services
{
    public partial class AdminService : IAdminService
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminService(ShopDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<SelectList> GetCategoriesSelectListAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return new SelectList(categories, "Id", "Name");
        }

        public async Task<(byte[] bytes, string contentType, string fileName)> ExportRevenueExcelAsync(string dateRange)
        {
            var stats = await GetDashboardAsync(dateRange);
            
            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Revenue");
            
            ws.Cells[1, 1].Value = "Period";
            ws.Cells[1, 2].Value = "Revenue";
            ws.Cells[1, 3].Value = "Expense";
            ws.Cells[1, 4].Value = "Profit";
            
            for (int i = 0; i < stats.Months.Count; i++)
            {
                ws.Cells[i + 2, 1].Value = stats.Months[i];
                ws.Cells[i + 2, 2].Value = stats.RevenueTrend[i];
                ws.Cells[i + 2, 3].Value = stats.CostTrend[i];
                ws.Cells[i + 2, 4].Value = stats.ProfitTrend[i];
            }
            
            return (package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Revenue.xlsx");
        }

        public async Task<(byte[] bytes, string contentType, string fileName)> ExportRevenueCsvAsync(string dateRange)
        {
            var stats = await GetDashboardAsync(dateRange);
            
            using var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream, leaveOpen: true);
            await using var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
            
            await csv.WriteRecordsAsync(stats.Months.Select((m, i) => new { Period = m, Revenue = stats.RevenueTrend[i], Expense = stats.CostTrend[i], Profit = stats.ProfitTrend[i] }));
            await writer.FlushAsync();
            
            return (stream.ToArray(), "text/csv", "Revenue.csv");
        }
    }
}