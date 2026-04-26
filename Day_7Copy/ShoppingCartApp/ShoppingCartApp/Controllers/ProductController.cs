// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;
using System.Text.Json;

namespace ShoppingCartApp.Controllers
{
    public partial class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ShopDbContext _context;
        private const string CompareSessionKey = "ProductCompareList";
        private const string RecentlyViewedSessionKey = "RecentlyViewedProducts";
        private const int MaxRecentlyViewed = 8;

        public ProductController(IProductService productService, ShopDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        // Track recently viewed product in session
        private void TrackRecentlyViewed(int productId)
        {
            var recentIdsStr = HttpContext.Session.GetString(RecentlyViewedSessionKey) ?? "[]";
            var recentIds = JsonSerializer.Deserialize<List<int>>(recentIdsStr) ?? new List<int>();
            
            // Remove if already exists (to move to top)
            recentIds.Remove(productId);
            
            // Add to beginning
            recentIds.Insert(0, productId);
            
            // Limit to max count
            if (recentIds.Count > MaxRecentlyViewed)
            {
                recentIds = recentIds.Take(MaxRecentlyViewed).ToList();
            }
            
            HttpContext.Session.SetString(RecentlyViewedSessionKey, JsonSerializer.Serialize(recentIds));
        }

        // GET: /Product
        public async Task<IActionResult> Index(
            string searchQuery = null,
            string category = null,
            int page = 1,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = null)
        {
            return Execute(await _productService.GetProductsAsync(searchQuery, category, page, 12, minPrice, maxPrice, sortBy));
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _productService.GetProductDetailsAsync(id);
            // Track only if product exists
            if (result.Succeeded)
            {
                TrackRecentlyViewed(id);
            }
            return Execute(result);
        }

        // Duplicate - actual implementation is in ProductController.Search.cs
        [NonAction]
        public async Task<IActionResult> Search(string query)
        {
            return Execute(await _productService.SearchProductsAsync(query));
        }

        // GET: /Product/Compare
        public async Task<IActionResult> Compare()
        {
            var compareIds = HttpContext.Session.GetString(CompareSessionKey);
            if (string.IsNullOrEmpty(compareIds))
            {
                return View(new List<Models.Product>());
            }

            var productIds = JsonSerializer.Deserialize<List<int>>(compareIds) ?? new List<int>();
            return Execute(await _productService.GetCompareProductsAsync(productIds));
        }

        // GET: /Product/AddToCompare/5
        public IActionResult AddToCompare(int id)
        {
            var compareIds = HttpContext.Session.GetString(CompareSessionKey);
            var productIds = string.IsNullOrEmpty(compareIds) 
                ? new List<int>() 
                : JsonSerializer.Deserialize<List<int>>(compareIds) ?? new List<int>();

            if (productIds.Count >= 4)
            {
                TempData["Warning"] = "You can compare up to 4 products only. Remove one to add another.";
                return RedirectToAction("Compare");
            }

            if (!productIds.Contains(id))
            {
                productIds.Add(id);
                HttpContext.Session.SetString(CompareSessionKey, JsonSerializer.Serialize(productIds));
                TempData["Success"] = "Product added to comparison list.";
            }
            else
            {
                TempData["Info"] = "Product is already in your comparison list.";
            }

            return RedirectToAction("Compare");
        }

        // GET: /Product/RemoveFromCompare/5
        public IActionResult RemoveFromCompare(int id)
        {
            var compareIds = HttpContext.Session.GetString(CompareSessionKey);
            if (!string.IsNullOrEmpty(compareIds))
            {
                var productIds = JsonSerializer.Deserialize<List<int>>(compareIds) ?? new List<int>();
                productIds.Remove(id);
                HttpContext.Session.SetString(CompareSessionKey, JsonSerializer.Serialize(productIds));
                TempData["Info"] = "Product removed from comparison.";
            }

            return RedirectToAction("Compare");
        }

        // GET: /Product/ClearCompare
        public IActionResult ClearCompare()
        {
            HttpContext.Session.Remove(CompareSessionKey);
            TempData["Info"] = "Comparison list cleared.";
            return RedirectToAction("Compare");
        }

        // API: /Product/GetRecommendations/{id}
        public async Task<IActionResult> GetRecommendations(int id)
        {
            return await _productService.GetRecommendationsApi(id);
        }

        // API: /Product/GetCompareCount
        public IActionResult GetCompareCount()
        {
            return _productService.GetCompareCountApi(HttpContext);
        }

        // API: /Product/GetRecentlyViewed
        public async Task<IActionResult> GetRecentlyViewed()
        {
            try
            {
                var recentIdsStr = HttpContext.Session.GetString(RecentlyViewedSessionKey);
                if (string.IsNullOrEmpty(recentIdsStr))
                {
                    return Json(new { success = true, products = new List<object>() });
                }

                var recentIds = JsonSerializer.Deserialize<List<int>>(recentIdsStr) ?? new List<int>();
                if (!recentIds.Any())
                {
                    return Json(new { success = true, products = new List<object>() });
                }

                // Get up to 7 recently viewed products (excluding current one which is first)
                var otherIds = recentIds.Skip(1).Take(7).ToList();

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .Where(p => otherIds.Contains(p.Id))
                    .ToListAsync();

                // Maintain the order from session
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

                return Json(new { success = true, products = orderedProducts });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: /Product/LoadMoreProducts?page=2&category=all&minPrice=100&maxPrice=500&sortBy=price_asc
        public async Task<IActionResult> LoadMoreProducts(int page = 1, string category = "all", decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null)
        {
            try
            {
                var result = await _productService.GetProductsAsync(null, category == "all" ? null : category, page, 12, minPrice, maxPrice, sortBy);
                if (result.ViewModel is ProductPaginationViewModel vm)
                {
                    return Json(new { 
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
                return Json(new { success = false, message = ex.Message });
            }
            return Json(new { success = false });
        }
    }
}