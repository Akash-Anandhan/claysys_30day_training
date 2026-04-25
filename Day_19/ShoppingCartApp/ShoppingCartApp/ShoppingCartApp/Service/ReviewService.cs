// Services/ReviewService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Review;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ShopDbContext _context;

        public ReviewService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse> AddReviewAsync(AddReviewDto dto)
        {
            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.UserId == dto.UserId && r.ProductId == dto.ProductId);

            if (alreadyReviewed)
                return new ServiceResponse
                {
                    Succeeded = true,
                    RedirectAction = "Details",
                    RedirectController = "Product",
                    RouteValues = new { id = dto.ProductId },
                    TempData = new Dictionary<string, string>
                    {
                        { "ReviewError", "You have already reviewed this product." }
                    }
                };

            _context.Reviews.Add(new Review
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Details",
                RedirectController = "Product",
                RouteValues = new { id = dto.ProductId },
                TempData = new Dictionary<string, string>
                {
                    { "ReviewSuccess", "Your review has been submitted!" }
                }
            };
        }

        public async Task<ServiceResponse> DeleteReviewAsync(DeleteReviewDto dto)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == dto.ReviewId && r.UserId == dto.UserId);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Details", "Product", new { id = dto.ProductId });
        }
    }
}
