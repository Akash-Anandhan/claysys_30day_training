using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync();
        Task<string> AddToCartAsync(AddToCartDto dto);
        Task<string> RemoveFromCartAsync(int id);
        Task<object> UpdateCartItemAsync(int id, UpdateCartDto dto);
    }
}
