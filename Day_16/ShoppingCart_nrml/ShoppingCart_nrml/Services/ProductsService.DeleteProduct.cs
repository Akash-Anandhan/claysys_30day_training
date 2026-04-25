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
        public async Task<string> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _cache.Remove($"product_{id}");
            _cache.Remove("products_all");

            return "Product deleted successfully";
        }
    }
}


