using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services.Interfaces;

using System.Data.Entity;
using ShoppingCartAPI.Data;

namespace ShoppingCartAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ShopDbContext _context;


        public ReviewService(ShopDbContext context)
        {
            _context = context;

        }

        public async Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {

            var dbReviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            var productReviews = dbReviews
                .Select(r => new ReviewDto
                {
                    Comment = r.Comment,
                    Rating = r.Rating
                })
                .ToList();

            return productReviews;
        }

        public async Task<ReviewDto> AddReviewAsync(int productId, CreateReviewDto dto)
        {

            // Optional: You could check if productId actually exists in Products table here.

            var newReview = new Review
            {
                ProductId = productId,
                Comment = dto.Comment,
                Rating = dto.Rating
            };

            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();


            return new ReviewDto
            {
                Comment = newReview.Comment,
                Rating = newReview.Rating
            };
        }
    }
}

