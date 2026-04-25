// Services/Interface/IAdminService.cs
using ShoppingCartApp.DTOs.Admin;

namespace ShoppingCartApp.Services
{
    public interface IAdminService
    {
        // Dashboard
        Task<AdminDashboardDto> GetDashboardAsync(string dateRange = "This Month");
        Task SeedDummyDataAsync();

        // Products
        Task<ServiceResponse> GetProductsAsync(
            string searchQuery = null,
            string category = null,
            string stockFilter = null,
            string sortBy = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 50);
        Task<ServiceResponse> GetCreateProductFormAsync();
        Task<ServiceResponse> CreateProductAsync(CreateProductDto dto);
        Task<ServiceResponse> GetEditProductFormAsync(int id);
        Task<ServiceResponse> EditProductAsync(EditProductDto dto);
        Task<ServiceResponse> DeleteProductAsync(int id);
        Task<ServiceResponse> DeleteAllProductsAsync();

        // Reviews
        Task<ServiceResponse> GetReviewsAsync(
            string searchQuery = null,
            int? minRating = null,
            string sortBy = null,
            int page = 1,
            int pageSize = 50);
        Task<ServiceResponse> DeleteReviewAsync(int id);

        // Orders
        Task<AdminOrdersDto> GetOrdersAsync(
            string searchQuery = null,
            string statusFilter = null,
            string sortBy = null,
            int page = 1,
            int pageSize = 50);
        Task<AdminOrderDetailDto> GetOrderDetailAsync(int id);
        Task<ServiceResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
        Task<(byte[] bytes, string contentType, string fileName)> ExportOrdersExcelAsync();
        Task<(byte[] bytes, string contentType, string fileName)> ExportOrdersCsvAsync();

        // Import / Export
        Task<(byte[] bytes, string contentType, string fileName)> ExportExcelAsync();
        Task<(byte[] bytes, string contentType, string fileName)> ExportCsvAsync();
        Task<ServiceResponse> ImportExcelAsync(ImportFileDto dto);
        Task<ServiceResponse> ImportCsvAsync(ImportFileDto dto);
        Task<ServiceResponse> GetUsersAsync();
        Task<ServiceResponse> DeleteUserAsync(string id);
    }
}