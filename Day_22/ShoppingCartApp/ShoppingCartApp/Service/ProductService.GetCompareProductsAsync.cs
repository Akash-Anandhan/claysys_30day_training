// Services/ProductService.GetCompareProductsAsync.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using System.Text.Json;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetCompareProductsAsync(List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return ServiceResponse.ShowView("Compare", new List<Product>());
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => productIds.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync();

            // Maintain order based on productIds
            var orderedProducts = productIds
                .Select(id => products.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .ToList();

            return ServiceResponse.ShowView("Compare", orderedProducts);
        }
    }
}