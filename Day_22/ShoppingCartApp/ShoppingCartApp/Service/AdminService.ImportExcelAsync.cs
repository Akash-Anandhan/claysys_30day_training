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
        public async Task<ServiceResponse> ImportExcelAsync(ImportFileDto dto)
        {
            if (dto.File == null || dto.File.Length <= 0)
                return ServiceResponse.Redirect("Products");
            using var stream = new MemoryStream();
            await dto.File.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.FirstOrDefault();
            if (ws == null)
                return ServiceResponse.Redirect("Products");
            for (int row = 2; row <= ws.Dimension.Rows; row++)
            {
                var name = ws.Cells[row, 2].Value?.ToString();
                if (string.IsNullOrEmpty(name) || name == "Name")
                    continue;
                var imageUrl = ws.Cells[row, 6].Value?.ToString();
                int categoryId = int.TryParse(ws.Cells[row, 7].Value?.ToString(), out var catId) ? catId : 1;
                var basePrice = decimal.TryParse(ws.Cells[row, 4].Value?.ToString(), out var p) ? p : 0;
                var sellingPrice = basePrice * 1.10m;
                _context.Products.Add(new Product { Name = name, Description = ws.Cells[row, 3].Value?.ToString(), BasePrice = basePrice, SellingPrice = sellingPrice, Stock = int.TryParse(ws.Cells[row, 5].Value?.ToString(), out var stock) ? stock : 0, CategoryId = categoryId, ImageUrl = string.IsNullOrEmpty(imageUrl) ? "https://via.placeholder.com/300" : imageUrl });
            }

            await _context.SaveChangesAsync();
            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Products",
                TempData = new Dictionary<string, string>
                {
                    {
                        "Success",
                        "Products imported successfully from Excel."
                    }
                }
            };
        }
    }
}