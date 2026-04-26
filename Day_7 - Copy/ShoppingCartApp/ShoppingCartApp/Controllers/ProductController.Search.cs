// Controllers/ProductController.Search.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    // Part 2 – search, filtering, and listing actions
    public partial class ProductController
    {
        // GET: /Product/Search
        public async Task<IActionResult> Search(
            string query, string category,
            decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            return Execute(await _productService.SearchAsync(query, category, minPrice, maxPrice, sortBy));
        }

        // GET: /Product/ByCategory/Electronics
        public async Task<IActionResult> ByCategory(string categoryName)
        {
            var response = await _productService.GetByCategoryAsync(categoryName);
            if (response.ViewModel == null)
                return NotFound();

            // Unpack so the view still receives a product list + ViewBag.CategoryName
            var dto = (ProductListDto)response.ViewModel;
            ViewBag.CategoryName = dto.CategoryName;
            return View("Index", dto.Products);
        }

        // GET: /Product/TopRated
        public async Task<IActionResult> TopRated()
        {
            var dto = (ProductListDto)(await _productService.GetTopRatedAsync()).ViewModel;
            ViewBag.PageTitle = dto.PageTitle;
            return View("Index", dto.Products);
        }

        // GET: /Product/NewArrivals
        public async Task<IActionResult> NewArrivals()
        {
            var dto = (ProductListDto)(await _productService.GetNewArrivalsAsync()).ViewModel;
            ViewBag.PageTitle = dto.PageTitle;
            return View("Index", dto.Products);
        }
    }
}
