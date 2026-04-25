// Services/ICartService.cs
using ShoppingCartApp.DTOs.Cart;

namespace ShoppingCartApp.Services
{
    public interface ICartService
    {
        Task<ServiceResponse> GetCartAsync(string userId);
        Task<ServiceResponse> AddToCartAsync(AddToCartDto dto);
        Task<ServiceResponse> RemoveFromCartAsync(RemoveFromCartDto dto);
        Task<UpdateQuantityResultDto> UpdateQuantityAsync(UpdateQuantityDto dto);
        Task<int> GetCartCountAsync(string userId);
        Task MergeGuestCartAsync(string guestId, string userId);
    }
}