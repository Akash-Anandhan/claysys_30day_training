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
        // ── Private helpers ────────────────────────────────────────────────────
        private static async Task<string> HandleImageAsync(ProductViewModel model, string webRootPath)
        {
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var folder = Path.Combine(webRootPath, "images", "products");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(folder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.ImageFile.CopyToAsync(stream);
                return $"/images/products/{fileName}";
            }

            return !string.IsNullOrEmpty(model.ImageUrl) ? model.ImageUrl : null;
        }
    }
}