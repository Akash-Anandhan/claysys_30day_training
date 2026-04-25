// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopDbContext _context;

        public ProductService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .ToListAsync();

            return ServiceResponse.ShowView("Index", products);
        }

        public async Task<ServiceResponse> GetDetailsAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);

            return ServiceResponse.ShowView("Details", product);
        }

        public async Task<ServiceResponse> SearchAsync(ProductSearchDto dto)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Query))
                query = query.Where(p =>
                    p.Name.Contains(dto.Query) ||
                    p.Description.Contains(dto.Query) ||
                    p.Category.Name.Contains(dto.Query));

            if (!string.IsNullOrWhiteSpace(dto.Category))
                query = query.Where(p => p.Category.Name == dto.Category);

            if (dto.MinPrice.HasValue)
                query = query.Where(p => p.Price >= dto.MinPrice.Value);

            if (dto.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= dto.MaxPrice.Value);

            query = dto.SortBy switch
            {
                "price_asc"  => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc"   => query.OrderBy(p => p.Name),
                "name_desc"  => query.OrderByDescending(p => p.Name),
                "newest"     => query.OrderByDescending(p => p.Id),
                "top_rated"  => query.OrderByDescending(p =>
                                    p.Reviews.Average(r => (double?)r.Rating) ?? 0),
                _            => query.OrderBy(p => p.Name)
            };

            var results    = await query.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            return ServiceResponse.ShowView("Index", new ProductSearchResultDto
            {
                Results    = results,
                Categories = categories,
                TotalResults = results.Count
            });
        }

        public async Task<ServiceResponse> GetByCategoryAsync(string categoryName)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.Name == categoryName);

            if (!exists)
                return ServiceResponse.ShowView("NotFound", null);

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Category.Name == categoryName)
                .ToListAsync();

            return ServiceResponse.ShowView("Index", new ProductListDto
            {
                Products = products,
                CategoryName = categoryName
            });
        }

        public async Task<ServiceResponse> GetTopRatedAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Reviews.Any())
                .ToListAsync();

            var sorted = products
                .OrderByDescending(p => p.Reviews.Average(r => r.Rating))
                .Take(10)
                .ToList();

            return ServiceResponse.ShowView("Index", new ProductListDto
            {
                Products = sorted,
                PageTitle = "Top Rated Products"
            });
        }

        public async Task<ServiceResponse> GetNewArrivalsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .Take(8)
                .ToListAsync();

            return ServiceResponse.ShowView("Index", new ProductListDto
            {
                Products = products,
                PageTitle = "New Arrivals"
            });
        }

        public async Task<ServiceResponse> GetSuggestionsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return ServiceResponse.ShowView(null, new List<object>());

            var suggestions = await _context.Products
                .Where(p => p.Name.Contains(query))
                .Select(p => new { p.Id, p.Name, p.Price })
                .Take(5)
                .ToListAsync();

            return ServiceResponse.ShowView(null, suggestions);
        }
    }
}
