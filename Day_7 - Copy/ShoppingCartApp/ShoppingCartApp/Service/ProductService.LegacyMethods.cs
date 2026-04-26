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
            var products = await GetProductsAsync(query, category, 1, 50);
            return products;
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