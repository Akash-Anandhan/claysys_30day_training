// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetAllAsync()
        {
            var products = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).ToListAsync();
            return ServiceResponse.ShowView("Index", products);
        }
    }
}