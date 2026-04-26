// Services/ProductService.ProductDetails.cs
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
        public async Task<ServiceResponse> GetProductDetailsAsync(int id)
        {
            string cacheKey = string.Format(CultureInfo.InvariantCulture, ProductDetailCacheKey, id);
            
            if (!_cache.TryGetValue(cacheKey, out object cachedResult))
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                        .ThenInclude(r => r.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return ServiceResponse.ShowError(ProductNotFoundError);

                cachedResult = product;
                _cache.Set(cacheKey, product, CacheDuration);
            }

            return ServiceResponse.ShowView(ProductDetailView, cachedResult);
        }

        public async Task<ServiceResponse> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetProductsAsync();

            var searchLower = query.ToLower();
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Name.ToLower().Contains(searchLower) ||
                           (p.Description != null && p.Description.ToLower().Contains(searchLower)))
                .AsNoTracking()
                .OrderByDescending(p => p.Id)
                .Take(20)
                .ToListAsync();

            var viewModel = new ProductPaginationViewModel
            {
                Products = products,
                Categories = await _context.Categories.OrderBy(c => c.Name).AsNoTracking().ToListAsync(),
                SearchQuery = query,
                CurrentPage = 1,
                TotalPages = 1,
                TotalProducts = products.Count
            };

            return ServiceResponse.ShowView(IndexView, viewModel);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        private const string ProductDetailCacheKey = "product_{0}";
        private const string ProductDetailView = "Details";
        private const string ProductNotFoundError = "Product not found";
    }
}