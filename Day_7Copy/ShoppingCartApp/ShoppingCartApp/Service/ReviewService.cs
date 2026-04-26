// Services/ReviewService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Review;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ReviewService : IReviewService
    {
        private readonly ShopDbContext _context;
        public ReviewService(ShopDbContext context)
        {
            _context = context;
        }
    }
}