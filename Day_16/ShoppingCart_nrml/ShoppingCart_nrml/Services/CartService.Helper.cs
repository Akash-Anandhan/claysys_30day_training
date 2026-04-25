using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Services
{
    public partial class CartService
    {
        private async Task<Product?> GetProductInfoAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }
    }
}

