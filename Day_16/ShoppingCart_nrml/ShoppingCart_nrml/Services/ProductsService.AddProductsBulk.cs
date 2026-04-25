using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
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

            _context.Products.AddRange(productEntities);
            await _context.SaveChangesAsync();

            _cache.Remove("products_all");

            return new
            {
                Message = "Products added successfully",
                Count = productEntities.Count
            };
        }
    }
}



