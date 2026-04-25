using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data.Entity;
using ShoppingCartAPI.Data;

namespace ShoppingCartAPI.Services
{
    public class OfferService : IOfferService
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<OfferService> _logger;

        public OfferService(ShopDbContext context, ILogger<OfferService> logger)
        {
            _context = context;
            _logger = logger;
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
            _logger.LogInformation("Fetching active offers for ProductId: {ProductId}", productId);

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

            _logger.LogInformation("Found {OfferCount} active offers for ProductId: {ProductId}", productOffers.Count, productId);
            return productOffers;
        }

        public async Task<OfferDto?> ApplyCouponAsync(string couponCode)
        {
            _logger.LogInformation("Applying coupon code: {CouponCode}", couponCode);

            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.CouponCode == couponCode && o.IsActive);

            if (offer == null)
            {
                _logger.LogWarning("Coupon code {CouponCode} is invalid or inactive.", couponCode);
                return null;
            }

            _logger.LogInformation("Coupon code {CouponCode} applied successfully with {DiscountPercentage}% discount.", couponCode, offer.DiscountPercentage);
            return new OfferDto
            {
                DiscountPercentage = offer.DiscountPercentage,
                CouponCode = offer.CouponCode
            };
        }

        public async Task<OfferDto> AddOfferAsync(int productId, CreateOfferDto dto)
        {
            _logger.LogInformation("Adding a new offer for ProductId: {ProductId}", productId);

            var newOffer = new Offer
            {
                ProductId = productId,
                DiscountPercentage = dto.DiscountPercentage,
                CouponCode = dto.CouponCode,
                IsActive = dto.IsActive
            };

            await _context.Offers.AddAsync(newOffer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully added offer with ID {OfferId} for ProductId {ProductId}", newOffer.Id, productId);

            return new OfferDto
            {
                DiscountPercentage = newOffer.DiscountPercentage,
                CouponCode = newOffer.CouponCode
            };
        }
    }
}
