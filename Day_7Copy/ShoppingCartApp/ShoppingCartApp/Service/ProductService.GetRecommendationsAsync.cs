// Services/ProductService.GetRecommendationsAsync.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShoppingCartApp.Models;
using System.Text.Json;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        // API endpoint for AJAX recommendation loading
        public async Task<IActionResult> GetRecommendationsApi(int productId, int count = 4)
        {
            var recommendations = await GetRecommendationsAsync(productId, count);
            return new JsonResult(recommendations.Select(p => new
            {
                p.Id,
                p.Name,
                p.ImageUrl,
                p.SellingPrice,
                CategoryName = p.Category?.Name
            }));
        }

        public async Task<List<Product>> GetRecommendationsAsync(int productId, int count = 4)
        {
            string cacheKey = string.Format(RecCacheKeyTemplate, productId);
            
            if (!_cache.TryGetValue(cacheKey, out object recommendations))
            {
                // Get the current product
                var currentProduct = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (currentProduct == null)
                    return new List<Product>();

                var result = new List<Product>();

                // Strategy 1: Same category products (prioritized)
                var sameCategoryQuery = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .Where(p => p.CategoryId == currentProduct.CategoryId && p.Id != productId)
                    .AsNoTracking();

                var sameCategory = await sameCategoryQuery
                    .OrderByDescending(p => p.Reviews.Count)
                    .Take(count)
                    .ToListAsync();
                result.AddRange(sameCategory);

                // Strategy 2: Products frequently bought together with current product
                var orderIdsWithCurrentProduct = await _context.OrderItems
                    .Where(oi => oi.ProductId == productId)
                    .Select(oi => oi.OrderId)
                    .Distinct()
                    .ToListAsync();

                if (orderIdsWithCurrentProduct.Any())
                {
                    var boughtTogetherProductIds = await _context.OrderItems
                        .Where(oi => orderIdsWithCurrentProduct.Contains(oi.OrderId) && oi.ProductId != productId)
                        .GroupBy(oi => oi.ProductId)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .Take(count)
                        .ToListAsync();

                    var boughtTogether = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Reviews)
                        .Where(p => boughtTogetherProductIds.Contains(p.Id))
                        .AsNoTracking()
                        .ToListAsync();
                    
                    // Add only if not already in result
                    foreach (var product in boughtTogether)
                    {
                        if (!result.Any(p => p.Id == product.Id) && result.Count < count * 2)
                            result.Add(product);
                    }
                }

                // Fill remaining slots with popular products
                if (result.Count < count)
                {
                    var existingIds = result.Select(p => p.Id).ToHashSet();
                    existingIds.Add(productId); // Exclude current product
                    
                    var popularProducts = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Reviews)
                        .Where(p => !existingIds.Contains(p.Id))
                        .OrderByDescending(p => p.Reviews.Count)
                        .Take(count - result.Count)
                        .AsNoTracking()
                        .ToListAsync();
                    result.AddRange(popularProducts);
                }

                recommendations = result.Take(count).ToList();
                _cache.Set(cacheKey, recommendations, ShortCacheDuration);
            }

            return (List<Product>)recommendations;
        }

        private const string RecCacheKeyTemplate = "recommendations_{0}";
    }
}