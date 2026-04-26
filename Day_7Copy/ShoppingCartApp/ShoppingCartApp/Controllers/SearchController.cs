// Controllers/SearchController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Models;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    public class SearchController : BaseController
    {
        private readonly IProductService _productService;

        public SearchController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /Search
        public async Task<IActionResult> Index(
            string query, string category,
            decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var response = await _productService.SearchAsync(query, category, minPrice, maxPrice, sortBy);
            
            // Get products from ProductPaginationViewModel
            var paginationVm = response.ViewModel as ProductPaginationViewModel;
            var products = paginationVm?.Products ?? Enumerable.Empty<Product>();
            var categories = paginationVm?.Categories ?? await _productService.GetCategoriesAsync();

            return View(new SearchViewModel
            {
                Query        = query,
                Category     = category,
                MinPrice     = minPrice,
                MaxPrice     = maxPrice,
                SortBy       = sortBy,
                Results      = products,
                Categories   = categories,
                TotalResults = products.Count()
            });
        }

        // GET: /Search/Suggestions (AJAX – live search)
        public async Task<IActionResult> Suggestions(string query)
        {
            var response = await _productService.GetSuggestionsAsync(query);
            return Json(response.ViewModel);
        }
    }
}
