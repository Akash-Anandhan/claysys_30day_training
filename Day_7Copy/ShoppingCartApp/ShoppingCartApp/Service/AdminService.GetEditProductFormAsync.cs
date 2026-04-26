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
        public async Task<ServiceResponse> GetEditProductFormAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);
            var categories = await _context.Categories.ToListAsync();
            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                SellingPrice = product.SellingPrice,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };
            return ServiceResponse.ShowView("EditProduct", new { Model = model, Categories = categories });
        }
    }
}