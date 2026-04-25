// DTOs/Review/ReviewDtos.cs
namespace ShoppingCartApp.DTOs.Review
{
    // ── Inbound DTOs (Controller → Service) ──

    public class AddReviewDto
    {
        public string UserId { get; set; }   // resolved from claims by controller
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class DeleteReviewDto
    {
        public string UserId { get; set; }
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
    }
}
