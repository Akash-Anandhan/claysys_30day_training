// Services/Interface/IProductService.cs
using ShoppingCartApp.DTOs.Product;

namespace ShoppingCartApp.Services
{
    public interface IProductService
    {
        Task<ServiceResponse> GetAllAsync();
        Task<ServiceResponse> GetDetailsAsync(int id);
        Task<ServiceResponse> SearchAsync(ProductSearchDto dto);
        Task<ServiceResponse> GetByCategoryAsync(string categoryName);
        Task<ServiceResponse> GetTopRatedAsync();
        Task<ServiceResponse> GetNewArrivalsAsync();
        Task<ServiceResponse> GetSuggestionsAsync(string query);
    }
}
