// Services/ProductService.LegacyMethods.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Services
{
    public partial class ProductService
    {
        // Legacy interface methods for backward compatibility
        public async Task<ServiceResponse> SearchAsync(string query, string category = null, decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null)
        {
            // Build query with filters
            var dbQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsNoTracking();

            // Apply search query filter
            if (!string.IsNullOrWhiteSpace(query))
            {
                var searchLower = query.ToLower();
                dbQuery = dbQuery.Where(p => 
                    p.Name.ToLower().Contains(searchLower) || 
                    (p.Description != null && p.Description.ToLower().Contains(searchLower)));
            }

            // Apply category filter (with null check for Category)
            if (!string.IsNullOrWhiteSpace(category))
            {
                dbQuery = dbQuery.Where(p => p.Category != null && p.Category.Name == category);
            }

            // Apply price filters
            if (minPrice.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.SellingPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.SellingPrice <= maxPrice.Value);
            }

            // Apply sorting
            dbQuery = sortBy switch
            {
                "price_asc" => dbQuery.OrderBy(p => p.SellingPrice),
                "price_desc" => dbQuery.OrderByDescending(p => p.SellingPrice),
                "name_asc" => dbQuery.OrderBy(p => p.Name),
                "name_desc" => dbQuery.OrderByDescending(p => p.Name),
                "newest" => dbQuery.OrderByDescending(p => p.Id),
                _ => dbQuery.OrderByDescending(p => p.Id)
            };

            var products = await dbQuery.ToListAsync();
            
            var viewModel = new ProductPaginationViewModel
            {
                Products = products,
                Categories = await GetCategoriesAsync(),
                SearchQuery = query,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                CurrentPage = 1,
                TotalPages = 1,
                TotalProducts = products.Count
            };

            return ServiceResponse.ShowView(IndexView, viewModel);
        }

        public async Task<ServiceResponse> GetByCategoryAsync(string categoryName)
        {
            return await GetProductsAsync(null, categoryName, 1, 12);
        }

        public async Task<ServiceResponse> GetTopRatedAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsNoTracking()
                .OrderByDescending(p => p.Reviews.Average(r => r.Rating))
                .Take(12)
                .ToListAsync();

            var viewModel = new ProductPaginationViewModel
            {
                Products = products,
                Categories = await GetCategoriesAsync(),
                CurrentPage = 1,
                TotalPages = 1,
                TotalProducts = products.Count
            };

            return ServiceResponse.ShowView(IndexView, viewModel);
        }

        public async Task<ServiceResponse> GetNewArrivalsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Take(12)
                .ToListAsync();

            var viewModel = new ProductPaginationViewModel
            {
                Products = products,
                Categories = await GetCategoriesAsync(),
                CurrentPage = 1,
                TotalPages = 1,
                TotalProducts = products.Count
            };

            return ServiceResponse.ShowView(IndexView, viewModel);
        }

        private const string IndexView = "Index";

        public async Task<ServiceResponse> GetSuggestionsAsync(string query)
        {
            var products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(query.ToLower()))
                .Take(10)
                .AsNoTracking()
                .ToListAsync();
            
            return ServiceResponse.ShowView("Index", products);
        }
    }
}