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
            int pageSize = 12,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = null)
        {
            string cacheKey = string.Format(
                CultureInfo.InvariantCulture,
                "products_search_{0}_cat_{1}_page_{2}_min_{3}_max_{4}_sort_{5}",
                searchQuery ?? "",
                category ?? "",
                page,
                minPrice ?? 0,
                maxPrice ?? 0,
                sortBy ?? "");
            
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

                // Apply price filters
                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.SellingPrice >= minPrice.Value);
                }
                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.SellingPrice <= maxPrice.Value);
                }

                // Get total count
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                // Apply sorting
                query = sortBy switch
                {
                    "price_asc" => query.OrderBy(p => p.SellingPrice),
                    "price_desc" => query.OrderByDescending(p => p.SellingPrice),
                    "name_asc" => query.OrderBy(p => p.Name),
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    _ => query.OrderByDescending(p => p.Id)
                };

                // Apply pagination
                var products = await query
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
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SortBy = sortBy,
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