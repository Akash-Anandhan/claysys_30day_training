using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderResponseDto>> GetOrdersAsync();
        Task<object> CheckoutAsync(CheckoutDto dto);
    }
}
