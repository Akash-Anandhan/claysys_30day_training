using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemDto>> GetWishlistAsync();
        Task<string> AddToWishlistAsync(AddWishlistDto dto);
        Task<string> RemoveFromWishlistAsync(int productId);
    }
}
