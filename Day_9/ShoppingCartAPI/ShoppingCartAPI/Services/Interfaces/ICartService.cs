using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemResponseDto>> GetCartAsync(string userId);
        Task<string> AddToCartAsync(string userId, AddToCartDto dto);
        Task<string> RemoveFromCartAsync(string userId, int id);
        Task<object> UpdateCartItemAsync(string userId, int id, UpdateCartDto dto);
    }
}
