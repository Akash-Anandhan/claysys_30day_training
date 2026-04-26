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

        // API endpoint for AJAX compare operations
        public async Task<IActionResult> AddToCompareApi(int productId, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            const string SessionKey = "ProductCompareList";
            
            var compareIdsStr = httpContext.Session.GetString(SessionKey);
            var productIds = string.IsNullOrEmpty(compareIdsStr)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(compareIdsStr) ?? new List<int>();

            if (productIds.Count >= 4)
            {
                return new JsonResult(new { success = false, message = "You can compare up to 4 products only. Please remove one first." });
            }

            if (!productIds.Contains(productId))
            {
                productIds.Add(productId);
                httpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(productIds));
                return new JsonResult(new { success = true, message = "Product added to comparison.", count = productIds.Count });
            }

            return new JsonResult(new { success = true, message = "Product already in comparison.", count = productIds.Count });
        }

        public async Task<IActionResult> RemoveFromCompareApi(int productId, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            const string SessionKey = "ProductCompareList";
            
            var compareIdsStr = httpContext.Session.GetString(SessionKey);
            if (!string.IsNullOrEmpty(compareIdsStr))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(compareIdsStr) ?? new List<int>();
                productIds.Remove(productId);
                httpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(productIds));
            }

            return new JsonResult(new { success = true, message = "Product removed from comparison." });
        }

        public IActionResult ClearCompareApi(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            const string SessionKey = "ProductCompareList";
            httpContext.Session.Remove(SessionKey);
            return new JsonResult(new { success = true, message = "Comparison list cleared." });
        }

        public IActionResult GetCompareCountApi(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            const string SessionKey = "ProductCompareList";
            var compareIdsStr = httpContext.Session.GetString(SessionKey);
            var count = 0;
            
            if (!string.IsNullOrEmpty(compareIdsStr))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(compareIdsStr);
                count = productIds?.Count ?? 0;
            }

            return new JsonResult(new { count });
        }
    }
}