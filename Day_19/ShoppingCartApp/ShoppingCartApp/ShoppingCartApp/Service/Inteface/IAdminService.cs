// Services/Interface/IAdminService.cs
using ShoppingCartApp.DTOs.Admin;

namespace ShoppingCartApp.Services
{
    public interface IAdminService
    {
        // Dashboard
        Task<AdminDashboardDto> GetDashboardAsync();

        // Products
        Task<ServiceResponse> GetProductsAsync();
        Task<ServiceResponse> GetCreateProductFormAsync();
        Task<ServiceResponse> CreateProductAsync(CreateProductDto dto);
        Task<ServiceResponse> GetEditProductFormAsync(int id);
        Task<ServiceResponse> EditProductAsync(EditProductDto dto);
        Task<ServiceResponse> DeleteProductAsync(int id);
        Task<ServiceResponse> DeleteAllProductsAsync();

        // Reviews
        Task<ServiceResponse> GetReviewsAsync();
        Task<ServiceResponse> DeleteReviewAsync(int id);

        // Orders
        Task<AdminOrdersDto> GetOrdersAsync();
        Task<AdminOrderDetailDto> GetOrderDetailAsync(int id);
        Task<ServiceResponse> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);

        // Import / Export
        Task<(byte[] bytes, string contentType, string fileName)> ExportExcelAsync();
        Task<(byte[] bytes, string contentType, string fileName)> ExportCsvAsync();
        Task<ServiceResponse> ImportExcelAsync(ImportFileDto dto);
        Task<ServiceResponse> ImportCsvAsync(ImportFileDto dto);
    }
}
