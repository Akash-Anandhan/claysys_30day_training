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
        public async Task<ProductDto> GetProductAsync(int id)
        {
            var cacheKey = $"product_{id}";

            // If cache hit -> return cached data
            var cachedProduct = _cache.Get(cacheKey) as ProductDto;
            if (cachedProduct != null)
            {
                return cachedProduct;
            }

            // If cache miss -> fetch from DB

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
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

            var cachePolicy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) };

            // Store in cache
            _cache.Set(cacheKey, dto, cachePolicy);

            return dto;
        }
    }
}


