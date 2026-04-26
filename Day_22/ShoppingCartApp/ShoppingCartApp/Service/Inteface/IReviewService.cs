// Services/Interface/IReviewService.cs
using ShoppingCartApp.DTOs.Review;

namespace ShoppingCartApp.Services
{
    public interface IReviewService
    {
        Task<ServiceResponse> AddReviewAsync(AddReviewDto dto);
        Task<ServiceResponse> DeleteReviewAsync(DeleteReviewDto dto);
    }
}
