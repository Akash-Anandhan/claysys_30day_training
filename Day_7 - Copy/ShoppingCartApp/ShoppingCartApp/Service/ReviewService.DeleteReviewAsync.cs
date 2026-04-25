// Services/ReviewService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Review;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ReviewService
    {
        public async Task<ServiceResponse> DeleteReviewAsync(DeleteReviewDto dto)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == dto.ReviewId && r.UserId == dto.UserId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Details", "Product", new { id = dto.ProductId });
        }
    }
}