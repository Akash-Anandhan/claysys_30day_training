using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using Microsoft.Extensions.Logging;
using System.Web;
using Microsoft.Extensions.Caching.Memory;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var cacheKey = "products_all";

            // If cache hit -> return cached data
            if (_cache.TryGetValue(cacheKey, out IEnumerable<ProductDto>? cachedProducts) && cachedProducts != null)
            {
                _logger.LogInformation("Products fetched from cache via key: {CacheKey}", cacheKey);
                return cachedProducts;
            }

            // If cache miss -> fetch from DB
            _logger.LogInformation("Retrieving products from database.");

            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var dtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            }).ToList();

            // Set expiration
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)) // Absolute expiration: 5 minutes
                .SetSlidingExpiration(TimeSpan.FromMinutes(2)); // Sliding expiration: 2 minutes

            // Store in cache
            _logger.LogInformation("Storing products in cache via key: {CacheKey}", cacheKey);
            _cache.Set(cacheKey, dtos, cacheOptions);

            return dtos;
        }
    }
}
