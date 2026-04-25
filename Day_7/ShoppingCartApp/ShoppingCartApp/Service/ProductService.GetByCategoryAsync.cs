// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetByCategoryAsync(string categoryName)
        {
            var exists = await _context.Categories.AnyAsync(c => c.Name == categoryName);
            if (!exists)
                return ServiceResponse.ShowView("NotFound", null);
            var products = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).Where(p => p.Category.Name == categoryName).ToListAsync();
            return ServiceResponse.ShowView("Index", new ProductListDto { Products = products, CategoryName = categoryName });
        }
    }
}