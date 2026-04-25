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
        public async Task<IEnumerable<ProductDto>> GetYouMayLikeAsync()
        {
            var userId = _userContextService.GetUserId();
            var cartCategoryIds = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Select(c => c.Product.CategoryId)
                .Distinct()
                .ToListAsync();

            if (!cartCategoryIds.Any())
            {
                return new List<ProductDto>();
            }

            var cartProductIds = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Select(c => c.ProductId)
                .ToListAsync();

            var recommendedProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => cartCategoryIds.Contains(p.CategoryId) && !cartProductIds.Contains(p.Id))
                .Take(5)
                .ToListAsync();

            return recommendedProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            });
        }
    }
}
