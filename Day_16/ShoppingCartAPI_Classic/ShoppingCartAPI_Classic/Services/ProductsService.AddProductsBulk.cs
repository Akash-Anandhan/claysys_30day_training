using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using Microsoft.Extensions.Logging;
using System.Web;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<object> AddProductsBulkAsync(List<ProductDto> products)
        {

            var productEntities = new List<Product>();

            foreach (var dto in products)
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    ImageUrl = dto.ImageUrl,
                    Stock = dto.Stock,
                    CategoryId = dto.CategoryId
                };

                productEntities.Add(product);
            }

            await _context.Products.AddRangeAsync(productEntities);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Invalidating products_all cache after bulk insert.");
            _cache.Remove("products_all");

            return new
            {
                Message = "Products added successfully",
                Count = productEntities.Count
            };
        }
    }
}
