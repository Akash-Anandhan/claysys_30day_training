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
        public async Task<ServiceResponse> CreateProductAsync(CreateProductDto dto)
        {
            var imageUrl = await HandleImageAsync(dto.Model, dto.WebRootPath);
            var sellingPrice = dto.Model.SellingPrice ?? dto.Model.BasePrice * 1.10m;
            _context.Products.Add(new Product { Name = dto.Model.Name, Description = dto.Model.Description, BasePrice = dto.Model.BasePrice, SellingPrice = sellingPrice, Stock = dto.Model.Stock, CategoryId = dto.Model.CategoryId, ImageUrl = imageUrl ?? "https://via.placeholder.com/300" });
            await _context.SaveChangesAsync();
            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Products",
                TempData = new Dictionary<string, string>
                {
                    {
                        "Success",
                        $"Product '{dto.Model.Name}' created successfully!"}
                }
            };
        }
    }
}