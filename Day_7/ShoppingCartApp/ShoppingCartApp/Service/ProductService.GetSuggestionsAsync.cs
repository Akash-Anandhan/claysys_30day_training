// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetSuggestionsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return ServiceResponse.ShowView(null, new List<object>());
            var suggestions = await _context.Products.Where(p => p.Name.Contains(query)).Select(p => new { p.Id, p.Name, p.SellingPrice }).Take(5).ToListAsync();
            return ServiceResponse.ShowView(null, suggestions);
        }
    }
}