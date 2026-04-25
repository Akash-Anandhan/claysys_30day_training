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
        public async Task<string> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Invalidating caches for product {Id} and products_all after deletion.", id);
            _cache.Remove($"product_{id}");
            _cache.Remove("products_all");

            return "Product deleted successfully";
        }
    }
}
