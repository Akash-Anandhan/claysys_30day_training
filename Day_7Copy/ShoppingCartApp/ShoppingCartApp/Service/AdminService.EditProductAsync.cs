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
        public async Task<ServiceResponse> EditProductAsync(EditProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);
            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);
            var imageUrl = await HandleImageAsync(dto.Model, dto.WebRootPath);
            product.Name = dto.Model.Name;
            product.Description = dto.Model.Description;
            product.BasePrice = dto.Model.BasePrice;
            product.SellingPrice = dto.Model.SellingPrice ?? dto.Model.BasePrice * 1.10m;
            product.Stock = dto.Model.Stock;
            product.CategoryId = dto.Model.CategoryId;
            if (!string.IsNullOrEmpty(imageUrl))
                product.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();
            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Products",
                TempData = new Dictionary<string, string>
                {
                    {
                        "Success",
                        $"Product '{product.Name}' updated successfully!"}
                }
            };
        }
    }
}