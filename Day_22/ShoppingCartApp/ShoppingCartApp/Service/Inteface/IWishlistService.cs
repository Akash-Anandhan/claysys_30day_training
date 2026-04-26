// Services/IWishlistService.cs
using ShoppingCartApp.DTOs.Wishlist;

namespace ShoppingCartApp.Services
{
    public interface IWishlistService
    {
        Task<ServiceResponse> GetWishlistAsync(string userId);
        Task<ServiceResponse> AddToWishlistAsync(AddToWishlistDto dto);
        Task<ServiceResponse> RemoveFromWishlistAsync(RemoveFromWishlistDto dto);
        Task<ServiceResponse> MoveToCartAsync(MoveToCartDto dto);
        Task<int> GetWishlistCountAsync(string userId);
        Task<ServiceResponse> RemoveFromWishlistByProductAsync(string userId, int productId);
    }
}