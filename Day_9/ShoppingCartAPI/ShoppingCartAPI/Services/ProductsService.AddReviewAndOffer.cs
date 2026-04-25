using ShoppingCartAPI.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService
    {
        public async Task<ReviewDto> AddReviewAsync(int id, CreateReviewDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == id);
            if (!productExists)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            return await _reviewService.AddReviewAsync(id, dto);
        }

        public async Task<OfferDto> AddOfferAsync(int id, CreateOfferDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == id);
            if (!productExists)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            return await _offerService.AddOfferAsync(id, dto);
        }
    }
}
