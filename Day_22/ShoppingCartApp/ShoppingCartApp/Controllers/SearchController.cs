// Controllers/SearchController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Services;

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
            var viewModel = await _productService.GetSearchViewModelAsync(query, category, minPrice, maxPrice, sortBy);
            return View(viewModel);
        }

        // GET: /Search/Suggestions (AJAX - live search)
        public async Task<IActionResult> Suggestions(string query)
        {
            var response = await _productService.GetSuggestionsAsync(query);
            return Json(response.ViewModel);
        }
    }
}