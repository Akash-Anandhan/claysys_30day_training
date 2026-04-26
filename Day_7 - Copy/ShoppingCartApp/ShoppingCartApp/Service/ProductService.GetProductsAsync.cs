// Services/ProductService.GetProductsAsync.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Globalization;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        public async Task<ServiceResponse> GetProductsAsync(
            string searchQuery = null,
            string category = null,
            int page = 1,
            int pageSize = 12)
        {
            string cacheKey = string.Format(
                CultureInfo.InvariantCulture,
                "products_search_{0}_cat_{1}_page_{2}",
                searchQuery ?? "",
                category ?? "",
                page);
            
            if (!_cache.TryGetValue(cacheKey, out object cachedResult))
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .AsNoTracking()
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    var searchLower = searchQuery.ToLower();
                    query = query.Where(p => 
                        p.Name.ToLower().Contains(searchLower) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchLower)));
                }

                // Apply category filter
                if (!string.IsNullOrWhiteSpace(category))
                {
                    query = query.Where(p => p.Category != null && p.Category.Name == category);
                }

                // Get total count
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Apply pagination
                var products = await query
                    .OrderByDescending(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Get categories for filter dropdown
                var categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .AsNoTracking()
                    .ToListAsync();

                var viewModel = new ProductPaginationViewModel
                {
                    Products = products,
                    Categories = categories,
                    SearchQuery = searchQuery,
                    Category = category,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalProducts = totalCount
                };

                cachedResult = viewModel;
                _cache.Set(cacheKey, viewModel, CacheDuration);
            }

            return ServiceResponse.ShowView("Index", cachedResult);
        }
    }
}