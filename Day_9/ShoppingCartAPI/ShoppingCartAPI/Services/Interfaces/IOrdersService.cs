using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(string userId);
        Task<object> CheckoutAsync(string userId, CheckoutDto dto);
    }
}
