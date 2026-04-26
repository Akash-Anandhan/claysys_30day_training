// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Services;
using ShoppingCartApp.Models;
using System.Text.Json;

namespace ShoppingCartApp.Controllers
{
    public partial class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private const string CompareSessionKey = "ProductCompareList";

        public ProductController(IProductService productService)
        {
            _productService = productService;
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
            var response = await _productService.GetProductsAsync(searchQuery, category, page, 12, minPrice, maxPrice, sortBy);
            return View(response.ViewModel);
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _productService.GetProductDetailsAsync(id);
            if (result.Succeeded)
            {
                _productService.TrackRecentlyViewed(id, HttpContext);
            }
            return View(result.ViewModel);
        }

        // GET: /Product/Compare
        public async Task<IActionResult> Compare()
        {
            var compareIds = HttpContext.Session.GetString(CompareSessionKey);
            if (string.IsNullOrEmpty(compareIds))
            {
                return View(new List<Product>());
            }

            var productIds = JsonSerializer.Deserialize<List<int>>(compareIds) ?? new List<int>();
            var result = await _productService.GetCompareProductsAsync(productIds);
            return View(result.ViewModel);
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
            return await _productService.GetRecentlyViewedApi(HttpContext);
        }

        // API: /Product/LoadMoreProducts
        public async Task<IActionResult> LoadMoreProducts(int page = 1, string category = "all", decimal? minPrice = null, decimal? maxPrice = null, string sortBy = null)
        {
            return await _productService.LoadMoreProductsApi(page, category, minPrice, maxPrice, sortBy);
        }
    }
}