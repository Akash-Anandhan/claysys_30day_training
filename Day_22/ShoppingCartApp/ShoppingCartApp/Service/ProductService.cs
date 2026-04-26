// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Globalization;
using System.Text.Json;

namespace ShoppingCartApp.Services
{
    public partial class ProductService : IProductService
    {
        private readonly ShopDbContext _context;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ShortCacheDuration = TimeSpan.FromMinutes(5);
        private const string CompareSessionKey = "ProductCompareList";
        private const string RecentlyViewedSessionKey = "RecentlyViewedProducts";
        private const int MaxRecentlyViewed = 8;

        public ProductService(ShopDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // Recently Viewed Methods
        public void TrackRecentlyViewed(int productId, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var recentIdsStr = httpContext.Session.GetString(RecentlyViewedSessionKey) ?? "[]";
            var recentIds = JsonSerializer.Deserialize<List<int>>(recentIdsStr) ?? new List<int>();
            
            recentIds.Remove(productId);
            recentIds.Insert(0, productId);
            
            if (recentIds.Count > MaxRecentlyViewed)
            {
                recentIds = recentIds.Take(MaxRecentlyViewed).ToList();
            }
            
            httpContext.Session.SetString(RecentlyViewedSessionKey, JsonSerializer.Serialize(recentIds));
        }

        public async Task<IActionResult> GetRecentlyViewedApi(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            try
            {
                var recentIdsStr = httpContext.Session.GetString(RecentlyViewedSessionKey);
                if (string.IsNullOrEmpty(recentIdsStr))
                {
                    return new JsonResult(new { success = true, products = new List<object>() });
                }

                var recentIds = JsonSerializer.Deserialize<List<int>>(recentIdsStr) ?? new List<int>();
                if (!recentIds.Any())
                {
                    return new JsonResult(new { success = true, products = new List<object>() });
                }

                var otherIds = recentIds.Skip(1).Take(7).ToList();

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .Where(p => otherIds.Contains(p.Id))
                    .ToListAsync();

                var orderedProducts = otherIds
                    .Select(id => products.FirstOrDefault(p => p.Id == id))
                    .Where(p => p != null)
                    .Select(p => new {
                        id = p.Id,
                        name = p.Name,
                        imageUrl = p.ImageUrl,
                        sellingPrice = p.SellingPrice,
                        basePrice = p.BasePrice,
                        categoryName = p.Category?.Name,
                        avgRating = p.Reviews?.Any() == true ? p.Reviews.Average(r => r.Rating) : 0,
                        reviewCount = p.Reviews?.Count() ?? 0
                    })
                    .ToList();

                return new JsonResult(new { success = true, products = orderedProducts });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // Compare API Methods
        public async Task<IActionResult> AddToCompareApi(int productId, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var compareIds = httpContext.Session.GetString(CompareSessionKey);
            var productIds = string.IsNullOrEmpty(compareIds) 
                ? new List<int>() 
                : JsonSerializer.Deserialize<List<int>>(compareIds) ?? new List<int>();

            if (productIds.Count >= 4)
            {
                return new JsonResult(new { success = false, message = "You can compare up to 4 products only. Remove one to add another." });
            }

            if (!productIds.Contains(productId))
            {
                productIds.Add(productId);
                httpContext.Session.SetString(CompareSessionKey, JsonSerializer.Serialize(productIds));
                return new JsonResult(new { success = true, message = "Product added to comparison list." });
            }
            else
            {
                return new JsonResult(new { success = true, message = "Product is already in your comparison list." });
            }
        }

        public async Task<IActionResult> RemoveFromCompareApi(int productId, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var compareIds = httpContext.Session.GetString(CompareSessionKey);
            if (!string.IsNullOrEmpty(compareIds))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(compareIds) ?? new List<int>();
                productIds.Remove(productId);
                httpContext.Session.SetString(CompareSessionKey, JsonSerializer.Serialize(productIds));
            }
            return new JsonResult(new { success = true, message = "Product removed from comparison." });
        }

        public IActionResult ClearCompareApi(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            httpContext.Session.Remove(CompareSessionKey);
            return new JsonResult(new { success = true, message = "Comparison list cleared." });
        }

        public IActionResult GetCompareCountApi(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            var compareIds = httpContext.Session.GetString(CompareSessionKey);
            var count = string.IsNullOrEmpty(compareIds) ? 0 : (JsonSerializer.Deserialize<List<int>>(compareIds) ?? new List<int>()).Count;
            return new JsonResult(new { count });
        }

        // Search View Model
        public async Task<SearchViewModel> GetSearchViewModelAsync(string query, string category, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var response = await SearchAsync(query, category, minPrice, maxPrice, sortBy);
            var paginationVm = response.ViewModel as ProductPaginationViewModel;
            var products = paginationVm?.Products ?? Enumerable.Empty<Product>();
            var categories = paginationVm?.Categories ?? await GetCategoriesAsync();

            return new SearchViewModel
            {
                Query = query,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                Results = products,
                Categories = categories,
                TotalResults = products.Count()
            };
        }

        // Load More Products (AJAX)
        public async Task<IActionResult> LoadMoreProductsApi(int page = 1, string category = "all", decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null)
        {
            try
            {
                var result = await GetProductsAsync(null, category == "all" ? null : category, page, 12, minPrice, maxPrice, sortBy);
                if (result.ViewModel is ProductPaginationViewModel vm)
                {
                    return new JsonResult(new { 
                        success = true, 
                        products = vm.Products.Select(p => new {
                            id = p.Id,
                            name = p.Name,
                            description = p.Description,
                            imageUrl = p.ImageUrl,
                            sellingPrice = p.SellingPrice,
                            basePrice = p.BasePrice,
                            categoryName = p.Category?.Name,
                            categoryId = p.CategoryId,
                            stock = p.Stock,
                            avgRating = p.Reviews?.Any() == true ? p.Reviews.Average(r => r.Rating) : 0,
                            reviewCount = p.Reviews?.Count() ?? 0
                        }),
                        hasMore = page < vm.TotalPages,
                        currentPage = vm.CurrentPage,
                        totalPages = vm.TotalPages
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
            return new JsonResult(new { success = false });
        }
    }
}