// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetTopRatedAsync()
        {
            var products = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).Where(p => p.Reviews.Any()).ToListAsync();
            var sorted = products.OrderByDescending(p => p.Reviews.Average(r => r.Rating)).Take(10).ToList();
            return ServiceResponse.ShowView("Index", new ProductListDto { Products = sorted, PageTitle = "Top Rated Products" });
        }
    }
}