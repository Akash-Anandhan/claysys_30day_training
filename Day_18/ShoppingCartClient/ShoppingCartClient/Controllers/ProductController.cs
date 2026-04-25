using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Services;
using System.Threading.Tasks;

namespace ShoppingCartClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly IApiService _apiService;

        public ProductController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _apiService.GetProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Search(string query)
        {
            var products = await _apiService.GetProductsAsync();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                products = products.Where(p => 
                    p.Name.ToLower().Contains(lowerQuery) || 
                    (p.Description != null && p.Description.ToLower().Contains(lowerQuery)) ||
                    p.CategoryName.ToLower().Contains(lowerQuery)).ToList();
            }
            
            ViewData["Title"] = $"Search Results for '{query}'";
            return View("Index", products);
        }
    }
}
