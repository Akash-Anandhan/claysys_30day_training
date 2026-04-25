using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderResponseDto>> GetOrdersAsync();
        Task<object> CheckoutAsync(CheckoutDto dto);
    }
}

