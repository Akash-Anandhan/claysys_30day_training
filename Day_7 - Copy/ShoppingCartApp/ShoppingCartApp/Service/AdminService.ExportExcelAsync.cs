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
        // ── Import / Export ────────────────────────────────────────────────────
        public async Task<(byte[] bytes, string contentType, string fileName)> ExportExcelAsync()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Products");
            ws.Cells[1, 1].Value = "Id";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "Description";
            ws.Cells[1, 4].Value = "Selling Price";
            ws.Cells[1, 5].Value = "Stock";
            ws.Cells[1, 6].Value = "Category";
            int row = 2;
            foreach (var p in products)
            {
                ws.Cells[row, 1].Value = p.Id;
                ws.Cells[row, 2].Value = p.Name;
                ws.Cells[row, 3].Value = p.Description;
                ws.Cells[row, 4].Value = p.SellingPrice;
                ws.Cells[row, 5].Value = p.Stock;
                ws.Cells[row, 6].Value = p.Category?.Name;
                row++;
            }

            return (package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
        }
    }
}