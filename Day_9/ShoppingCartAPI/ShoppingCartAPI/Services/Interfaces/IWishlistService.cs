using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemDto>> GetWishlistAsync(string userId);
        Task<string> AddToWishlistAsync(string userId, AddWishlistDto dto);
        Task<string> RemoveFromWishlistAsync(string userId, int productId);
    }
}
