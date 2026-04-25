using ShoppingCartClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ShoppingCartClient.Services.ApiService;

namespace ShoppingCartClient.Services
{
    public interface IApiService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<bool> UpdateProductAsync(ProductDto productDto);
        Task<bool> AddToCartAsync(AddToCartDto addToCartDto);
        Task<bool> RemoveFromCartAsync(int id);
        Task<UpdateCartResponse> UpdateCartItemAsync(int id, UpdateCartDto updateCartDto);
        Task<CartDto?> GetCartAsync();
        
        // Wishlist
        Task<List<WishlistItemDto>> GetWishlistAsync();
        Task<bool> AddToWishlistAsync(AddWishlistDto addWishlistDto);
        Task<bool> RemoveFromWishlistAsync(int productId);

        // Checkout
        Task<int> CheckoutAsync(CheckoutDto checkoutDto);

        // Admin Methods
        Task<List<UserProfileDto>> GetUsersAsync();
        Task<List<OrderResponseDto>> GetOrdersAsync();
        Task<byte[]> DownloadOrdersExportAsync(string format);
        Task<byte[]> DownloadProductsExportAsync(string format);
        Task<string> ImportProductsAsync(Microsoft.AspNetCore.Http.IFormFile file, string format);
    }
}
