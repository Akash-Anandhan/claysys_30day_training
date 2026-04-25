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
        public async Task<ProductDto> GetProductAsync(int id)
        {
            var cacheKey = $"product_{id}";

            // If cache hit -> return cached data
            if (_cache.TryGetValue(cacheKey, out ProductDto? cachedProduct) && cachedProduct != null)
            {
                _logger.LogInformation("Product id {Id} fetched from cache via key: {CacheKey}", id, cacheKey);
                return cachedProduct;
            }

            // If cache miss -> fetch from DB
            _logger.LogInformation("Retrieving product id {Id} from database.", id);

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                _logger.LogWarning("Product id {Id} not found in database. Avoiding caching null.", id);
                throw new KeyNotFoundException("Product not found");
            }

            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)) // Absolute expiration: 5 minutes
                .SetSlidingExpiration(TimeSpan.FromMinutes(2)); // Sliding expiration: 2 minutes

            // Store in cache
            _logger.LogInformation("Storing product id {Id} in cache via key: {CacheKey}", id, cacheKey);
            _cache.Set(cacheKey, dto, cacheOptions);

            return dto;
        }
    }
}
