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
        public async Task<ServiceResponse> ImportCsvAsync(ImportFileDto dto)
        {
            if (dto.File == null || dto.File.Length <= 0)
                return ServiceResponse.Redirect("Products");
            using var reader = new StreamReader(dto.File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<dynamic>().ToList();
            var defaultCategory = await _context.Categories.FirstOrDefaultAsync();
            foreach (var record in records)
            {
                var dict = (IDictionary<string, object>)record;
                if (!dict.ContainsKey("Name"))
                    continue;
                var basePrice = dict.ContainsKey("TargetPrice") && decimal.TryParse(dict["TargetPrice"]?.ToString(), out var tp) ? tp : (dict.ContainsKey("Price") && decimal.TryParse(dict["Price"]?.ToString(), out var p) ? p : 0);
                var sellingPrice = basePrice * 1.10m;
                _context.Products.Add(new Product { Name = dict["Name"]?.ToString(), Description = dict.ContainsKey("Description") ? dict["Description"]?.ToString() : null, BasePrice = basePrice, SellingPrice = sellingPrice, Stock = dict.ContainsKey("Stock") && int.TryParse(dict["Stock"]?.ToString(), out var stock) ? stock : 0, CategoryId = defaultCategory?.Id ?? 1, ImageUrl = "https://via.placeholder.com/300" });
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
                        "Products imported successfully from CSV."
                    }
                }
            };
        }
    }
}