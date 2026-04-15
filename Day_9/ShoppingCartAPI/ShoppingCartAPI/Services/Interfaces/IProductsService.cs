using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetYouMayLikeAsync(string userId);
        Task<ProductDto> PostProductAsync(ProductDto productDto);
        Task<string> PutProductAsync(int id, ProductDto productDto);
        Task<string> DeleteProductAsync(int id);
        Task<object> AddProductsBulkAsync(List<ProductDto> products);
    }
}
