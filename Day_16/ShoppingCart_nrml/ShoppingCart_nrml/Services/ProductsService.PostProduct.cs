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
        public async Task<ProductDto> PostProductAsync(ProductDto productDto)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == productDto.CategoryId);

            if (!categoryExists)
            {
                throw new Exception("Invalid CategoryId. Category does not exist.");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.Id = product.Id;

            _cache.Remove("products_all");

            return productDto;
        }
    }
}


