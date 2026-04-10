using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly ShopDbContext _context;

        public SearchController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: /Search
        public async Task<IActionResult> Index(
            string query,
            string category,
            decimal? minPrice,
            decimal? maxPrice,
            string sortBy)
        {
            // Start with all products
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsQueryable();

            // Filter by search query
            if (!string.IsNullOrWhiteSpace(query))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(query) ||
                    p.Description.Contains(query) ||
                    p.Category.Name.Contains(query));
            }

            // Filter by category
            if (!string.IsNullOrWhiteSpace(category))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Category.Name == category);
            }

            // Filter by min price
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p =>
                    p.Price >= minPrice.Value);
            }

            // Filter by max price
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p =>
                    p.Price <= maxPrice.Value);
            }

            // Sort results
            productsQuery = sortBy switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "name_asc" => productsQuery.OrderBy(p => p.Name),
                "name_desc" => productsQuery.OrderByDescending(p => p.Name),
                "newest" => productsQuery.OrderByDescending(p => p.Id),
                _ => productsQuery.OrderBy(p => p.Name)
            };

            var results = await productsQuery.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            var viewModel = new SearchViewModel
            {
                Query = query,
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                Results = results,
                Categories = categories,
                TotalResults = results.Count
            };

            return View(viewModel);
        }

        // GET: /Search/Suggestions (for live search)
        public async Task<IActionResult> Suggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Json(new List<string>());

            var suggestions = await _context.Products
                .Where(p => p.Name.Contains(query))
                .Select(p => new { p.Id, p.Name, p.Price })
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }
    }
}