// Services/Interface/IProductService.cs
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ShoppingCartApp.Services
{
    public interface IProductService
    {
        // Pagination & Caching
        Task<ServiceResponse> GetProductsAsync(string searchQuery = null, string category = null, int page = 1, int pageSize = 12, decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null);
        Task<ServiceResponse> GetProductDetailsAsync(int id);
        Task<ServiceResponse> SearchProductsAsync(string query);
        Task<ServiceResponse> SearchAsync(string query, string category = null, decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null);
        Task<ServiceResponse> GetByCategoryAsync(string categoryName);
        Task<ServiceResponse> GetTopRatedAsync();
        Task<ServiceResponse> GetNewArrivalsAsync();
        Task<List<Category>> GetCategoriesAsync();
        Task<ServiceResponse> GetSuggestionsAsync(string query);

        // Product Recommendations
        Task<List<Product>> GetRecommendationsAsync(int productId, int count = 4);
        Task<IActionResult> GetRecommendationsApi(int productId, int count = 4);

        // Recently Viewed
        void TrackRecentlyViewed(int productId, HttpContext httpContext);
        Task<IActionResult> GetRecentlyViewedApi(HttpContext httpContext);

        // Compare Feature
        Task<ServiceResponse> GetCompareProductsAsync(List<int> productIds);
        Task<IActionResult> AddToCompareApi(int productId, Microsoft.AspNetCore.Http.HttpContext httpContext);
        Task<IActionResult> RemoveFromCompareApi(int productId, Microsoft.AspNetCore.Http.HttpContext httpContext);
        IActionResult ClearCompareApi(Microsoft.AspNetCore.Http.HttpContext httpContext);
        IActionResult GetCompareCountApi(Microsoft.AspNetCore.Http.HttpContext httpContext);

        // Search View Model
        Task<SearchViewModel> GetSearchViewModelAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string sortBy);

        // Load More Products (AJAX)
        Task<IActionResult> LoadMoreProductsApi(int page = 1, string category = "all", decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null);

        // Cache Invalidation
        void InvalidateProductsCache();
        void InvalidateProductDetailCache(int productId);
    }
}