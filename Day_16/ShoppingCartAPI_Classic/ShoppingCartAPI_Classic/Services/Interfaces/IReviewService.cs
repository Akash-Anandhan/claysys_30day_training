using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task<ReviewDto> AddReviewAsync(int productId, CreateReviewDto dto);
    }
}
