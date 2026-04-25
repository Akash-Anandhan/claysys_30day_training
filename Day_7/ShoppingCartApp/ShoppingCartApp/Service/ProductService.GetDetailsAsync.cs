// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetDetailsAsync(int id)
        {
            var product = await _context.Products.Include(p => p.Category).Include(p => p.Reviews).ThenInclude(r => r.User).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);
            return ServiceResponse.ShowView("Details", product);
        }
    }
}