// Services/AdminService.cs
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ShoppingCartApp.DTOs.Admin;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;
using System.Globalization;

namespace ShoppingCartApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(ShopDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ── Dashboard ──────────────────────────────────────────────────────────

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            return new AdminDashboardDto
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders   = await _context.Orders.CountAsync(),
                TotalUsers    = await _userManager.Users.CountAsync(),
                TotalReviews  = await _context.Reviews.CountAsync(),
                RecentOrders  = await _context.Orders
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToListAsync(),
                LowStockProducts = await _context.Products
                    .Where(p => p.Stock < 5)
                    .ToListAsync()
            };
        }

        // ── Products ───────────────────────────────────────────────────────────

        public async Task<ServiceResponse> GetProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return ServiceResponse.ShowView("Products", products);
        }

        public async Task<ServiceResponse> GetCreateProductFormAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return ServiceResponse.ShowView("CreateProduct",
                new { Model = new ProductViewModel(), Categories = categories });
        }

        public async Task<ServiceResponse> CreateProductAsync(CreateProductDto dto)
        {
            var imageUrl = await HandleImageAsync(dto.Model, dto.WebRootPath);

            _context.Products.Add(new Product
            {
                Name        = dto.Model.Name,
                Description = dto.Model.Description,
                Price       = dto.Model.Price,
                Stock       = dto.Model.Stock,
                CategoryId  = dto.Model.CategoryId,
                ImageUrl    = imageUrl ?? "https://via.placeholder.com/300"
            });

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded        = true,
                RedirectAction   = "Products",
                TempData         = new Dictionary<string, string>
                {
                    { "Success", $"Product '{dto.Model.Name}' created successfully!" }
                }
            };
        }

        public async Task<ServiceResponse> GetEditProductFormAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);

            var categories = await _context.Categories.ToListAsync();
            var model = new ProductViewModel
            {
                Id          = product.Id,
                Name        = product.Name,
                Description = product.Description,
                Price       = product.Price,
                Stock       = product.Stock,
                CategoryId  = product.CategoryId,
                ImageUrl    = product.ImageUrl
            };

            return ServiceResponse.ShowView("EditProduct",
                new { Model = model, Categories = categories });
        }

        public async Task<ServiceResponse> EditProductAsync(EditProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.Id);
            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);

            var imageUrl = await HandleImageAsync(dto.Model, dto.WebRootPath);

            product.Name        = dto.Model.Name;
            product.Description = dto.Model.Description;
            product.Price       = dto.Model.Price;
            product.Stock       = dto.Model.Stock;
            product.CategoryId  = dto.Model.CategoryId;

            if (!string.IsNullOrEmpty(imageUrl))
                product.ImageUrl = imageUrl;

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded      = true,
                RedirectAction = "Products",
                TempData       = new Dictionary<string, string>
                {
                    { "Success", $"Product '{product.Name}' updated successfully!" }
                }
            };
        }

        public async Task<ServiceResponse> DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.CartItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return ServiceResponse.ShowView("NotFound", null);

            _context.Reviews.RemoveRange(product.Reviews);
            _context.CartItems.RemoveRange(product.CartItems);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded      = true,
                RedirectAction = "Products",
                TempData       = new Dictionary<string, string>
                {
                    { "Success", $"Product '{product.Name}' deleted successfully!" }
                }
            };
        }

        public async Task<ServiceResponse> DeleteAllProductsAsync()
        {
            _context.CartItems.RemoveRange(await _context.CartItems.ToListAsync());
            _context.Reviews.RemoveRange(await _context.Reviews.ToListAsync());
            _context.OrderItems.RemoveRange(await _context.OrderItems.ToListAsync());
            _context.Products.RemoveRange(await _context.Products.ToListAsync());
            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded      = true,
                RedirectAction = "Products",
                TempData       = new Dictionary<string, string>
                {
                    { "Success", "All products deleted successfully!" }
                }
            };
        }

        // ── Reviews ────────────────────────────────────────────────────────────

        public async Task<ServiceResponse> GetReviewsAsync()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return ServiceResponse.ShowView("Reviews", reviews);
        }

        public async Task<ServiceResponse> DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return new ServiceResponse
            {
                Succeeded      = true,
                RedirectAction = "Reviews",
                TempData       = new Dictionary<string, string>
                {
                    { "Success", "Review deleted successfully!" }
                }
            };
        }

        // ── Orders ─────────────────────────────────────────────────────────────

        public async Task<AdminOrdersDto> GetOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var userIds = orders.Select(o => o.UserId).Distinct().ToList();
            var userEmails = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            return new AdminOrdersDto { Orders = orders, UserEmails = userEmails };
        }

        public async Task<AdminOrderDetailDto> GetOrderDetailAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            var user = await _userManager.FindByIdAsync(order.UserId);
            return new AdminOrderDetailDto { Order = order, UserEmail = user?.Email };
        }

        public async Task<ServiceResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order != null)
            {
                order.Status = dto.Status;
                await _context.SaveChangesAsync();
            }

            return new ServiceResponse
            {
                Succeeded        = true,
                RedirectAction   = "OrderDetails",
                RouteValues      = new { id = dto.OrderId },
                TempData         = new Dictionary<string, string>
                {
                    { "Success", $"Order #{dto.OrderId} status updated to {dto.Status}." }
                }
            };
        }

        // ── Import / Export ────────────────────────────────────────────────────

        public async Task<(byte[] bytes, string contentType, string fileName)> ExportExcelAsync()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Products");

            ws.Cells[1, 1].Value = "Id";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "Description";
            ws.Cells[1, 4].Value = "Price";
            ws.Cells[1, 5].Value = "Stock";
            ws.Cells[1, 6].Value = "Category";

            int row = 2;
            foreach (var p in products)
            {
                ws.Cells[row, 1].Value = p.Id;
                ws.Cells[row, 2].Value = p.Name;
                ws.Cells[row, 3].Value = p.Description;
                ws.Cells[row, 4].Value = p.Price;
                ws.Cells[row, 5].Value = p.Stock;
                ws.Cells[row, 6].Value = p.Category?.Name;
                row++;
            }

            return (package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Products.xlsx");
        }

        public async Task<(byte[] bytes, string contentType, string fileName)> ExportCsvAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id, p.Name, p.Description, p.Price, p.Stock,
                    Category = p.Category != null ? p.Category.Name : ""
                })
                .ToListAsync();

            using var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream, leaveOpen: true);
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            await csv.WriteRecordsAsync(products);
            await writer.FlushAsync();

            return (stream.ToArray(), "text/csv", "Products.csv");
        }

        public async Task<ServiceResponse> ImportExcelAsync(ImportFileDto dto)
        {
            if (dto.File == null || dto.File.Length <= 0)
                return ServiceResponse.Redirect("Products");

            using var stream = new MemoryStream();
            await dto.File.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.FirstOrDefault();
            if (ws == null)
                return ServiceResponse.Redirect("Products");

            for (int row = 2; row <= ws.Dimension.Rows; row++)
            {
                var name = ws.Cells[row, 2].Value?.ToString();
                if (string.IsNullOrEmpty(name) || name == "Name") continue;

                var imageUrl   = ws.Cells[row, 6].Value?.ToString();
                int categoryId = int.TryParse(ws.Cells[row, 7].Value?.ToString(), out var catId) ? catId : 1;

                _context.Products.Add(new Product
                {
                    Name        = name,
                    Description = ws.Cells[row, 3].Value?.ToString(),
                    Price       = decimal.TryParse(ws.Cells[row, 4].Value?.ToString(), out var price) ? price : 0,
                    Stock       = int.TryParse(ws.Cells[row, 5].Value?.ToString(), out var stock) ? stock : 0,
                    CategoryId  = categoryId,
                    ImageUrl    = string.IsNullOrEmpty(imageUrl) ? "https://via.placeholder.com/300" : imageUrl
                });
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded      = true,
                RedirectAction = "Products",
                TempData       = new Dictionary<string, string>
                {
                    { "Success", "Products imported successfully from Excel." }
                }
            };
        }

        public async Task<ServiceResponse> ImportCsvAsync(ImportFileDto dto)
        {
            if (dto.File == null || dto.File.Length <= 0)
                return ServiceResponse.Redirect("Products");

            using var reader = new StreamReader(dto.File.OpenReadStream());
            using var csv    = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records         = csv.GetRecords<dynamic>().ToList();
            var defaultCategory = await _context.Categories.FirstOrDefaultAsync();

            foreach (var record in records)
            {
                var dict = (IDictionary<string, object>)record;
                if (!dict.ContainsKey("Name")) continue;

                _context.Products.Add(new Product
                {
                    Name        = dict["Name"]?.ToString(),
                    Description = dict.ContainsKey("Description") ? dict["Description"]?.ToString() : null,
                    Price       = dict.ContainsKey("Price") && decimal.TryParse(dict["Price"]?.ToString(), out var price) ? price : 0,
                    Stock       = dict.ContainsKey("Stock") && int.TryParse(dict["Stock"]?.ToString(), out var stock) ? stock : 0,
                    CategoryId  = defaultCategory?.Id ?? 1,
                    ImageUrl    = "https://via.placeholder.com/300"
                });
            }

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded      = true,
                RedirectAction = "Products",
                TempData       = new Dictionary<string, string>
                {
                    { "Success", "Products imported successfully from CSV." }
                }
            };
        }

        // ── Private helpers ────────────────────────────────────────────────────

        private static async Task<string> HandleImageAsync(ProductViewModel model, string webRootPath)
        {
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var folder   = Path.Combine(webRootPath, "images", "products");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.ImageFile.CopyToAsync(stream);

                return $"/images/products/{fileName}";
            }

            return !string.IsNullOrEmpty(model.ImageUrl) ? model.ImageUrl : null;
        }
    }
}
