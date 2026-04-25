// Controllers/ReviewController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Review;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class ReviewController : BaseController
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST: /Review/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Product", new { id = model.ProductId });

            return Execute(await _reviewService.AddReviewAsync(new AddReviewDto
            {
                UserId = GetUserId(),
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment
            }));
        }

        // POST: /Review/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            return Execute(await _reviewService.DeleteReviewAsync(new DeleteReviewDto
            {
                UserId = GetUserId(),
                ReviewId = id,
                ProductId = productId
            }));
        }
    }
}
