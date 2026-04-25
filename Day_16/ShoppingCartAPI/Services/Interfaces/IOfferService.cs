using ShoppingCartAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Services
{
    public interface IOfferService
    {
        Task<List<string>> GetOffersAsync();
        Task<List<OfferDto>> GetOffersByProductIdAsync(int productId);
        Task<OfferDto?> ApplyCouponAsync(string couponCode);
        Task<OfferDto> AddOfferAsync(int productId, CreateOfferDto dto);
    }
}
