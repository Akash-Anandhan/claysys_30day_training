using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductAsync(int id);
        Task<ProductDetailsDto> GetProductDetailsAsync(int id);
        Task<IEnumerable<ProductDto>> GetYouMayLikeAsync();
        Task<ProductDto> PostProductAsync(ProductDto productDto);
        Task<string> PutProductAsync(int id, ProductDto productDto);
        Task<string> DeleteProductAsync(int id);
        Task<object> AddProductsBulkAsync(List<ProductDto> products);
        Task<ReviewDto> AddReviewAsync(int id, CreateReviewDto dto);
        Task<OfferDto> AddOfferAsync(int id, CreateOfferDto dto);
    }
}
