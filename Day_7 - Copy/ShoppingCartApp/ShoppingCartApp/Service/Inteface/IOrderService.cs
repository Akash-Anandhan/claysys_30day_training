// Services/IOrderService.cs
using ShoppingCartApp.DTOs.Order;

namespace ShoppingCartApp.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse> CheckoutAsync(CheckoutDto dto);
        Task<ServiceResponse> PlaceOrderAsync(PlaceOrderDto dto);
        Task<ServiceResponse> GetConfirmationAsync(OrderConfirmationDto dto);
        Task<ServiceResponse> GetUserOrdersAsync(string userId);
        Task<ServiceResponse> CancelOrderAsync(int orderId, string userId);
    }
}