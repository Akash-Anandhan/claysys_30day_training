using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<string> PutProductAsync(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
                throw new ArgumentException("Product ID mismatch");

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.ImageUrl = productDto.ImageUrl;
            product.Stock = productDto.Stock;
            product.CategoryId = productDto.CategoryId;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                    throw new KeyNotFoundException("Product not found");
                else
                    throw;
            }

            _logger.LogInformation("Invalidating caches for product {Id} and products_all after update.", id);
            _cache.Remove($"product_{id}");
            _cache.Remove("products_all");

            return "Product updated successfully";
        }
    }
}
