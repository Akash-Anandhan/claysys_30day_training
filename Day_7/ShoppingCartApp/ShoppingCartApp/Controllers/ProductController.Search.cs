using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Controllers
{
    public partial class ProductController : Controller
    {
        // GET: /Product/Search?query=laptop&category=Electronics
        public async Task<IActionResult> Search(
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

            // Filter by category name
            if (!string.IsNullOrWhiteSpace(category))
            {
                productsQuery = productsQuery
                    .Where(p => p.Category.Name == category);
            }

            // Filter by minimum price
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery
                    .Where(p => p.Price >= minPrice.Value);
            }

            // Filter by maximum price
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery
                    .Where(p => p.Price <= maxPrice.Value);
            }

            // Sort results
            productsQuery = sortBy switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "name_asc" => productsQuery.OrderBy(p => p.Name),
                "name_desc" => productsQuery.OrderByDescending(p => p.Name),
                "top_rated" => productsQuery.OrderByDescending(
                                    p => p.Reviews.Average(
                                        r => (double?)r.Rating) ?? 0),
                _ => productsQuery.OrderBy(p => p.Name)
            };

            var results = await productsQuery.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            // Pass data to view via ViewBag
            ViewBag.Query = query;
            ViewBag.Category = category;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;
            ViewBag.Categories = categories;
            ViewBag.Total = results.Count;

            return View(results);
        }

        // GET: /Product/ByCategory/Electronics
        public async Task<IActionResult> ByCategory(string categoryName)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == categoryName);

            if (category == null)
                return NotFound();

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Category.Name == categoryName)
                .ToListAsync();

            ViewBag.CategoryName = categoryName;
            return View("Index", products);
        }

        // GET: /Product/TopRated
        public async Task<IActionResult> TopRated()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Reviews.Any())
                .ToListAsync();

            // Sort by average rating in memory
            var sorted = products
                .OrderByDescending(p =>
                    p.Reviews.Average(r => r.Rating))
                .Take(10)
                .ToList();

            ViewBag.PageTitle = "Top Rated Products";
            return View("Index", sorted);
        }

        // GET: /Product/NewArrivals
        public async Task<IActionResult> NewArrivals()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .Take(8)
                .ToListAsync();

            ViewBag.PageTitle = "New Arrivals";
            return View("Index", products);
        }
    }
}