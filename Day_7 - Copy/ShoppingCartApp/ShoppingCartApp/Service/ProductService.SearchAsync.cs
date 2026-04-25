// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> SearchAsync(ProductSearchDto dto)
        {
            var query = _context.Products.Include(p => p.Category).Include(p => p.Reviews).AsQueryable();
            if (!string.IsNullOrWhiteSpace(dto.Query))
                query = query.Where(p => p.Name.Contains(dto.Query) || p.Description.Contains(dto.Query) || p.Category.Name.Contains(dto.Query));
            if (!string.IsNullOrWhiteSpace(dto.Category))
                query = query.Where(p => p.Category.Name == dto.Category);
            if (dto.MinPrice.HasValue)
                query = query.Where(p => p.SellingPrice >= dto.MinPrice.Value);
            if (dto.MaxPrice.HasValue)
                query = query.Where(p => p.SellingPrice <= dto.MaxPrice.Value);
            query = dto.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.SellingPrice),
                "price_desc" => query.OrderByDescending(p => p.SellingPrice),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.Id),
                "top_rated" => query.OrderByDescending(p => p.Reviews.Average(r => (double? )r.Rating) ?? 0),
                _ => query.OrderBy(p => p.Name)};
            var results = await query.ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            return ServiceResponse.ShowView("Search", new ProductSearchResultDto { Results = results, Categories = categories, TotalResults = results.Count });
        }
    }
}