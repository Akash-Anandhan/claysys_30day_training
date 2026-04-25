// Controllers/SearchController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Product;
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
            var result = (ProductSearchResultDto)(await _productService.SearchAsync(new ProductSearchDto
            {
                Query    = query,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy   = sortBy
            })).ViewModel;

            return View(new SearchViewModel
            {
                Query        = query,
                Category     = category,
                MinPrice     = minPrice,
                MaxPrice     = maxPrice,
                SortBy       = sortBy,
                Results      = result.Results,
                Categories   = result.Categories,
                TotalResults = result.TotalResults
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
