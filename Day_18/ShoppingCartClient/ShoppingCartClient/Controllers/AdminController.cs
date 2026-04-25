using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Models;
using ShoppingCartClient.Services;
using System.Threading.Tasks;

namespace ShoppingCartClient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IApiService _apiService;

        public AdminController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _apiService.GetProductsAsync();
            var orders = await _apiService.GetOrdersAsync();
            var users = await _apiService.GetUsersAsync();

            var stats = new
            {
                TotalProducts = products.Count,
                TotalOrders = orders.Count,
                TotalUsers = users.Count,
                RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList()
            };

            return View(stats);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _apiService.GetUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _apiService.GetOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Products()
        {
            var products = await _apiService.GetProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> ExportProducts(string format = "excel")
        {
            var fileBytes = await _apiService.DownloadProductsExportAsync(format);
            if (fileBytes == null)
                return NotFound("Export failed.");

            var contentType = format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";
            var fileName = format == "excel" ? "Products.xlsx" : "Products.csv";

            return File(fileBytes, contentType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportOrders(string format = "excel")
        {
            var fileBytes = await _apiService.DownloadOrdersExportAsync(format);
            if (fileBytes == null)
                return NotFound("Export failed.");

            var contentType = format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";
            var fileName = format == "excel" ? "Orders.xlsx" : "Orders.csv";

            return File(fileBytes, contentType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _apiService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
                
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var success = await _apiService.UpdateProductAsync(model);
                if (success)
                {
                    TempData["Message"] = "Product updated successfully.";
                    return RedirectToAction("Products");
                }
                ModelState.AddModelError("", "Failed to update the product. Please check the values or try again later.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImportProducts(IFormFile file, string format)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid file.";
                return RedirectToAction("Products");
            }

            var result = await _apiService.ImportProductsAsync(file, format);
            TempData["Message"] = result;
            return RedirectToAction("Products");
        }
    }
}
