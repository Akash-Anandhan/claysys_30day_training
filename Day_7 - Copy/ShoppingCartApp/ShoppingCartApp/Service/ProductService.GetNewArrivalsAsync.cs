// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetNewArrivalsAsync()
        {
            var products = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).OrderByDescending(p => p.Id).Take(8).ToListAsync();
            return ServiceResponse.ShowView("Index", new ProductListDto { Products = products, PageTitle = "New Arrivals" });
        }
    }
}