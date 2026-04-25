using System;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data.Entity;
using ShoppingCartAPI.Data;

namespace ShoppingCartAPI.Services
{
    public class OfferService : IOfferService
    {
        private readonly ShopDbContext _context;


        public OfferService(ShopDbContext context)
        {
            _context = context;

        }

        public async Task<List<string>> GetOffersAsync()
        {
            // Simulate processing delay
            await Task.Delay(50);
            return new List<string>
            {
                "10% off on all Electronics items!",
                "Buy 2 get 1 free on Clothing.",
                "Free shipping on orders above $50!"
            };
        }

        public async Task<List<OfferDto>> GetOffersByProductIdAsync(int productId)
        {

            var dbOffers = await _context.Offers
                .Where(o => o.ProductId == productId && o.IsActive)
                .ToListAsync();

            var productOffers = dbOffers
                .Select(o => new OfferDto
                {
                    DiscountPercentage = o.DiscountPercentage,
                    CouponCode = o.CouponCode
                })
                .ToList();

            return productOffers;
        }

        public async Task<OfferDto?> ApplyCouponAsync(string couponCode)
        {

            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.CouponCode == couponCode && o.IsActive);

            if (offer == null)
            {
                return null;
            }

            return new OfferDto
            {
                DiscountPercentage = offer.DiscountPercentage,
                CouponCode = offer.CouponCode
            };
        }

        public async Task<OfferDto> AddOfferAsync(int productId, CreateOfferDto dto)
        {

            var newOffer = new Offer
            {
                ProductId = productId,
                DiscountPercentage = dto.DiscountPercentage,
                CouponCode = dto.CouponCode,
                IsActive = dto.IsActive
            };

            _context.Offers.Add(newOffer);
            await _context.SaveChangesAsync();


            return new OfferDto
            {
                DiscountPercentage = newOffer.DiscountPercentage,
                CouponCode = newOffer.CouponCode
            };
        }
    }
}

