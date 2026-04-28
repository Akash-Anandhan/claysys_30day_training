// Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;
using ShoppingCartApp.DTOs.Admin;

namespace ShoppingCartApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: /Admin
        public async Task<IActionResult> Index(string dateRange = "This Month")
        {
            ViewBag.CurrentDateRange = dateRange;
            var dto = await _adminService.GetDashboardAsync(dateRange);
            return View(dto);
        }

        // GET: /Admin/Revenue
        public async Task<IActionResult> Revenue(string dateRange = "This Year")
        {
            ViewBag.CurrentDateRange = dateRange;
            var dto = await _adminService.GetDashboardAsync(dateRange);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SeedData()
        {
            await _adminService.SeedDummyDataAsync();
            TempData["Success"] = "Dummy data generation successfully executed.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PopulatePaymentMethods()
        {
            var count = await _adminService.PopulatePaymentMethodsAsync();
            TempData["Success"] = $"Payment methods populated for {count} orders.";
            return RedirectToAction("Index");
        }

        // Products
        public async Task<IActionResult> Products(
            string searchQuery = null,
            string category = null,
            string stockFilter = null,
            string sortBy = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1)
        {
            var result = await _adminService.GetProductsAsync(searchQuery, category, stockFilter, sortBy, minPrice, maxPrice, page);
            ViewBag.Categories = await _adminService.GetCategoriesSelectListAsync();
            return View(result.ViewModel);
        }

        public async Task<IActionResult> CreateProduct()
        {
            var result = await _adminService.GetCreateProductFormAsync();
            ViewBag.Categories = await _adminService.GetCategoriesSelectListAsync();
            return View(result.ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = string.Join(" | ", ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}"));

                ViewBag.Categories = await _adminService.GetCategoriesSelectListAsync();
                return View(model);
            }

            var dto = new CreateProductDto { Model = model };
            var result = await _adminService.CreateProductAsync(dto);
            return ExecuteServiceResponse(result);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var result = await _adminService.GetEditProductFormAsync(id);
            if (result.ViewModel == null) return NotFound();

            ViewBag.Categories = await _adminService.GetCategoriesSelectListAsync();
            return View(result.ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, ProductViewModel model)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _adminService.GetCategoriesSelectListAsync();
                return View(model);
            }

            var dto = new EditProductDto { Id = id, Model = model };
            var result = await _adminService.EditProductAsync(dto);
            
            if (result.ViewModel == null && !result.Succeeded)
                return NotFound();

            return ExecuteServiceResponse(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _adminService.DeleteProductAsync(id);
            if (result.ViewModel == null && !result.Succeeded)
                return NotFound();

            return ExecuteServiceResponse(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllProducts()
        {
            var result = await _adminService.DeleteAllProductsAsync();
            return ExecuteServiceResponse(result);
        }

        // Reviews
        public async Task<IActionResult> Reviews(
            string searchQuery = null,
            int? minRating = null,
            string sortBy = null,
            int page = 1)
        {
            var result = await _adminService.GetReviewsAsync(searchQuery, minRating, sortBy, page);
            
            ViewBag.SearchQuery = searchQuery;
            ViewBag.MinRating = minRating;
            ViewBag.SortBy = sortBy;
            
            if (result.ViewModel is AdminReviewsResultDto dto)
            {
                ViewBag.RatingDistribution = dto.RatingDistribution;
                return View(dto);
            }
            
            ViewBag.RatingDistribution = new List<int> { 0, 0, 0, 0, 0 };
            return View(result.ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _adminService.DeleteReviewAsync(id);
            return ExecuteServiceResponse(result);
        }

        // Orders
        public async Task<IActionResult> Orders(
            string searchQuery = null,
            string statusFilter = null,
            string sortBy = null,
            int page = 1)
        {
            var dto = await _adminService.GetOrdersAsync(searchQuery, statusFilter, sortBy, page);
            ViewBag.UserEmails = dto.UserEmails;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.SortBy = sortBy;
            ViewBag.CurrentPage = dto.CurrentPage;
            ViewBag.TotalPages = dto.TotalPages;
            ViewBag.TotalCount = dto.TotalCount;
            ViewBag.StatusLabels = dto.StatusLabels;
            ViewBag.StatusCounts = dto.StatusCounts;
            ViewBag.AOVLabels = dto.AOVLabels;
            ViewBag.AOVValues = dto.AOVValues;
            return View(dto.Orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var dto = await _adminService.GetOrderDetailAsync(id);
            if (dto == null) return NotFound();

            ViewBag.UserEmail = dto.UserEmail;
            return View(dto.Order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var dto = new UpdateOrderStatusDto { OrderId = id, Status = status };
            var result = await _adminService.UpdateOrderStatusAsync(dto);
            return ExecuteServiceResponse(result);
        }

        // Export
        public async Task<IActionResult> ExportExcel(DateTime? startDate, DateTime? endDate)
        {
            if (startDate > DateTime.Now || endDate > DateTime.Now)
            {
                TempData["Error"] = "Cannot select future dates for export.";
                return RedirectToAction("Products");
            }
            if (startDate > endDate)
            {
                TempData["Error"] = "Start date cannot be later than end date.";
                return RedirectToAction("Products");
            }

            var (bytes, contentType, fileName) = await _adminService.ExportExcelAsync(startDate, endDate);
            return File(bytes, contentType, fileName);
        }

        public async Task<IActionResult> ExportCsv(DateTime? startDate, DateTime? endDate)
        {
            if (startDate > DateTime.Now || endDate > DateTime.Now)
            {
                TempData["Error"] = "Cannot select future dates for export.";
                return RedirectToAction("Products");
            }
            if (startDate > endDate)
            {
                TempData["Error"] = "Start date cannot be later than end date.";
                return RedirectToAction("Products");
            }

            var (bytes, contentType, fileName) = await _adminService.ExportCsvAsync(startDate, endDate);
            return File(bytes, contentType, fileName);
        }

        public async Task<IActionResult> ExportOrdersExcel(DateTime? startDate, DateTime? endDate)
        {
            if (startDate > DateTime.Now || endDate > DateTime.Now)
            {
                TempData["Error"] = "Cannot select future dates for export.";
                return RedirectToAction("Orders");
            }
            if (startDate > endDate)
            {
                TempData["Error"] = "Start date cannot be later than end date.";
                return RedirectToAction("Orders");
            }

            var (bytes, contentType, fileName) = await _adminService.ExportOrdersExcelAsync(startDate, endDate);
            return File(bytes, contentType, fileName);
        }

        public async Task<IActionResult> ExportOrdersCsv(DateTime? startDate, DateTime? endDate)
        {
            if (startDate > DateTime.Now || endDate > DateTime.Now)
            {
                TempData["Error"] = "Cannot select future dates for export.";
                return RedirectToAction("Orders");
            }
            if (startDate > endDate)
            {
                TempData["Error"] = "Start date cannot be later than end date.";
                return RedirectToAction("Orders");
            }

            var (bytes, contentType, fileName) = await _adminService.ExportOrdersCsvAsync(startDate, endDate);
            return File(bytes, contentType, fileName);
        }

        public async Task<IActionResult> ExportRevenueExcel()
        {
            var dateRange = ViewBag.CurrentDateRange as string ?? "This Year";
            var (bytes, contentType, fileName) = await _adminService.ExportRevenueExcelAsync(dateRange);
            return File(bytes, contentType, fileName);
        }

        public async Task<IActionResult> ExportRevenueCsv()
        {
            var dateRange = ViewBag.CurrentDateRange as string ?? "This Year";
            var (bytes, contentType, fileName) = await _adminService.ExportRevenueCsvAsync(dateRange);
            return File(bytes, contentType, fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var dto = new ImportFileDto { File = file };
            var result = await _adminService.ImportExcelAsync(dto);
            return ExecuteServiceResponse(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            var dto = new ImportFileDto { File = file };
            var result = await _adminService.ImportCsvAsync(dto);
            return ExecuteServiceResponse(result);
        }

        public async Task<IActionResult> Users(string searchQuery = null)
        {
            var result = await _adminService.GetUsersAsync(searchQuery);
            ViewBag.SearchQuery = searchQuery;
            return ExecuteServiceResponse(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            return ExecuteServiceResponse(result);
        }

        public async Task<IActionResult> ProductPerformance(
            string searchQuery = null,
            string category = null,
            string sortBy = null,
            int page = 1)
        {
            var result = await _adminService.GetProductPerformanceAsync(searchQuery, category, sortBy, page);
            ViewBag.SearchQuery = searchQuery;
            ViewBag.Category = category;
            ViewBag.SortBy = sortBy;
            return View(result);
        }
    }
}