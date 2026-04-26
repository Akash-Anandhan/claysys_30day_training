// Services/IOrderService.cs
using ShoppingCartApp.DTOs.Order;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse> CheckoutAsync(CheckoutDto dto);
        Task<ServiceResponse> PlaceOrderAsync(PlaceOrderDto dto);
        Task<ServiceResponse> GetConfirmationAsync(OrderConfirmationDto dto);
        Task<ServiceResponse> GetUserOrdersAsync(string userId, string? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 5);
        Task<ServiceResponse> CancelOrderAsync(int orderId, string userId);
        Task<Order?> GetOrderByIdAsync(int orderId, string userId);
    }
}