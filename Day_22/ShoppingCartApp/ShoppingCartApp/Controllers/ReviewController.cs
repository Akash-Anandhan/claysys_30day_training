// Controllers/ReviewController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;
using ShoppingCartApp.DTOs.Review;

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

            var dto = new AddReviewDto
            {
                UserId = GetUserId(),
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment
            };
            return ExecuteServiceResponse(await _reviewService.AddReviewAsync(dto));
        }

        // POST: /Review/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            var dto = new DeleteReviewDto
            {
                UserId = GetUserId(),
                ReviewId = id,
                ProductId = productId
            };
            return ExecuteServiceResponse(await _reviewService.DeleteReviewAsync(dto));
        }
    }
}