using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public AdminController(
            ShopDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // =====================
        // DASHBOARD
        // =====================

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                RecentOrders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync(),
                LowStockProducts = await _context.Products
                    .Where(p => p.Stock < 5)
                    .ToListAsync()
            };

            return View(stats);
        }

        // =====================
        // PRODUCTS
        // =====================

        // GET: /Admin/Products
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(products);
        }

        // GET: /Admin/CreateProduct
        public async Task<IActionResult> CreateProduct()
        {
            await PopulateCategoriesAsync();
            return View(new ProductViewModel());
        }

        // POST: /Admin/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            // Remove image fields from validation
            // since they are both optional
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                // Show exactly which fields are failing
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}");

                TempData["Error"] = string.Join(" | ", errors);
                await PopulateCategoriesAsync();
                return View(model);
            }

            // Make sure at least one image option is provided
            // otherwise use placeholder
            var imageUrl = await HandleImageAsync(model);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryId,
                ImageUrl = imageUrl ?? "https://via.placeholder.com/300"
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' created successfully!";
            return RedirectToAction("Products");
        }

        // GET: /Admin/EditProduct/5
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            await PopulateCategoriesAsync();

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };

            return View(model);
        }

        // POST: /Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, ProductViewModel model)
        {
            // Remove image fields from validation
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync();
                return View(model);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var imageUrl = await HandleImageAsync(model);

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.CategoryId = model.CategoryId;

            if (!string.IsNullOrEmpty(imageUrl))
                product.ImageUrl = imageUrl;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' updated successfully!";
            return RedirectToAction("Products");
        }

        // POST: /Admin/DeleteProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.CartItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            _context.Reviews.RemoveRange(product.Reviews);
            _context.CartItems.RemoveRange(product.CartItems);
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Product '{product.Name}' deleted successfully!";
            return RedirectToAction("Products");
        }


        // GET: /Admin/Reviews
        public async Task<IActionResult> Reviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        // POST: /Admin/DeleteReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Review deleted successfully!";
            }

            return RedirectToAction("Reviews");
        }

  

        private async Task PopulateCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        private async Task<string> HandleImageAsync(ProductViewModel model)
        {
            // If a file was uploaded, save it and return the path
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath, "images", "products");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() +
                    Path.GetExtension(model.ImageFile.FileName);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                return $"/images/products/{fileName}";
            }

            // Otherwise return the URL if provided
            if (!string.IsNullOrEmpty(model.ImageUrl))
                return model.ImageUrl;

            return null;
        }
    }
}