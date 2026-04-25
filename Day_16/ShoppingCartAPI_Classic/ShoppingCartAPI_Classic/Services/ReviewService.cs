using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data.Entity;
using ShoppingCartAPI.Data;

namespace ShoppingCartAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(ShopDbContext context, ILogger<ReviewService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            _logger.LogInformation("Fetching reviews for ProductId: {ProductId} from DB", productId);

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

            _logger.LogInformation("Found {ReviewCount} reviews for ProductId: {ProductId}", productReviews.Count, productId);
            return productReviews;
        }

        public async Task<ReviewDto> AddReviewAsync(int productId, CreateReviewDto dto)
        {
            _logger.LogInformation("Adding a new review for ProductId: {ProductId}", productId);

            // Optional: You could check if productId actually exists in Products table here.

            var newReview = new Review
            {
                ProductId = productId,
                Comment = dto.Comment,
                Rating = dto.Rating
            };

            await _context.Reviews.AddAsync(newReview);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully added review with ID {ReviewId} for ProductId {ProductId}", newReview.Id, productId);

            return new ReviewDto
            {
                Comment = newReview.Comment,
                Rating = newReview.Rating
            };
        }
    }
}
