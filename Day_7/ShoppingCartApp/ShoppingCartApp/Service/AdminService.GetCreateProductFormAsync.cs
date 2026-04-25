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
        public async Task<ServiceResponse> GetCreateProductFormAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return ServiceResponse.ShowView("CreateProduct", new { Model = new ProductViewModel(), Categories = categories });
        }
    }
}