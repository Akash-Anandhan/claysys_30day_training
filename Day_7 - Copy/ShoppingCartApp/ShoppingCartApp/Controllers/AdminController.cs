// Controllers/AdminController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Admin;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;
        private readonly IWebHostEnvironment _environment;

        public AdminController(IAdminService adminService, IWebHostEnvironment environment)
        {
            _adminService = adminService;
            _environment  = environment;
        }

        // â”€â”€ Dashboard â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        // GET: /Admin
        public async Task<IActionResult> Index(string dateRange = "This Month")
        {
            ViewBag.CurrentDateRange = dateRange;
            return View(await _adminService.GetDashboardAsync(dateRange));
        }

        // GET: /Admin/Revenue
        public async Task<IActionResult> Revenue(string dateRange = "This Year")
        {
            ViewBag.CurrentDateRange = dateRange;
            return View(await _adminService.GetDashboardAsync(dateRange));
        }

        [HttpPost]
        public async Task<IActionResult> SeedData()
        {
            await _adminService.SeedDummyDataAsync();
            TempData["Success"] = "Dummy data generation successfully executed.";
            return RedirectToAction("Index");
        }

        // â”€â”€ Products â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        // GET: /Admin/Products
        public async Task<IActionResult> Products()
        {
            return Execute(await _adminService.GetProductsAsync());
        }

        // GET: /Admin/CreateProduct
        public async Task<IActionResult> CreateProduct()
        {
            var result = await _adminService.GetCreateProductFormAsync();
            PopulateCategories(result);
            return View((ProductViewModel)((dynamic)result.ViewModel).Model);
        }

        // POST: /Admin/CreateProduct
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

                var form = await _adminService.GetCreateProductFormAsync();
                PopulateCategories(form);
                return View(model);
            }

            return Execute(await _adminService.CreateProductAsync(new CreateProductDto
            {
                Model       = model,
                WebRootPath = _environment.WebRootPath
            }));
        }

        // GET: /Admin/EditProduct/5
        public async Task<IActionResult> EditProduct(int id)
        {
            var result = await _adminService.GetEditProductFormAsync(id);
            if (result.ViewModel == null) return NotFound();

            PopulateCategories(result);
            return View((ProductViewModel)((dynamic)result.ViewModel).Model);
        }

        // POST: /Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, ProductViewModel model)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                var form = await _adminService.GetEditProductFormAsync(id);
                PopulateCategories(form);
                return View(model);
            }

            var response = await _adminService.EditProductAsync(new EditProductDto
            {
                Id          = id,
                Model       = model,
                WebRootPath = _environment.WebRootPath
            });

            if (response.ViewModel == null && !response.Succeeded)
                return NotFound();

            return Execute(response);
        }

        // POST: /Admin/DeleteProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _adminService.DeleteProductAsync(id);
            if (response.ViewModel == null && !response.Succeeded)
                return NotFound();

            return Execute(response);
        }

        // POST: /Admin/DeleteAllProducts
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllProducts()
        {
            return Execute(await _adminService.DeleteAllProductsAsync());
        }

        // â”€â”€ Reviews â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        // GET: /Admin/Reviews
        public async Task<IActionResult> Reviews()
        {
            return Execute(await _adminService.GetReviewsAsync());
        }

        // POST: /Admin/DeleteReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            return Execute(await _adminService.DeleteReviewAsync(id));
        }

        // â”€â”€ Orders â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        // GET: /Admin/Orders
        public async Task<IActionResult> Orders()
        {
            var dto = await _adminService.GetOrdersAsync();
            ViewBag.UserEmails = dto.UserEmails;
            return View(dto.Orders);
        }

        // GET: /Admin/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            var dto = await _adminService.GetOrderDetailAsync(id);
            if (dto == null) return NotFound();

            ViewBag.UserEmail = dto.UserEmail;
            return View(dto.Order);
        }

        // POST: /Admin/UpdateOrderStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            return Execute(await _adminService.UpdateOrderStatusAsync(new UpdateOrderStatusDto
            {
                OrderId = id,
                Status  = status
            }));
        }

        // â”€â”€ Import / Export â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        // GET: /Admin/ExportExcel
        public async Task<IActionResult> ExportExcel()
        {
            var (bytes, contentType, fileName) = await _adminService.ExportExcelAsync();
            return File(bytes, contentType, fileName);
        }

        // GET: /Admin/ExportCsv
        public async Task<IActionResult> ExportCsv()
        {
            var (bytes, contentType, fileName) = await _adminService.ExportCsvAsync();
            return File(bytes, contentType, fileName);
        }

        // POST: /Admin/ImportExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            return Execute(await _adminService.ImportExcelAsync(new ImportFileDto { File = file }));
        }

        // POST: /Admin/ImportCsv
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            return Execute(await _adminService.ImportCsvAsync(new ImportFileDto { File = file }));
        }

        // â”€â”€ Private helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        // IWebHostEnvironment is HTTP-infrastructure â€” stays in the controller.
        // Categories SelectList is view-prep â€” also stays here.
        private void PopulateCategories(ServiceResponse result)
        {
            var categories = ((dynamic)result.ViewModel).Categories;
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            return Execute(await _adminService.GetUsersAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            return Execute(await _adminService.DeleteUserAsync(id));
        }
    }
}
