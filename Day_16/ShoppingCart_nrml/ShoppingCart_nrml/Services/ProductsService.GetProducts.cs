using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using System.Web;
using System.Runtime.Caching;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var cacheKey = "products_all";

            // If cache hit -> return cached data
            var cachedProducts = _cache.Get(cacheKey) as IEnumerable<ProductDto>;
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            // If cache miss -> fetch from DB

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
            var cachePolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) };

            // Store in cache
            _cache.Set(cacheKey, dtos, cachePolicy);

            return dtos;
        }
    }
}


