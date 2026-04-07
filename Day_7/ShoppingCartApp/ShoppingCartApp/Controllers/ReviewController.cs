using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ShopDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: /Review/Add
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Product", new { id = model.ProductId });

            string userId = _userManager.GetUserId(User);

            // Check if user already reviewed this product
            var existing = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == model.ProductId);

            if (existing != null)
            {
                TempData["ReviewError"] = "You have already reviewed this product.";
                return RedirectToAction("Details", "Product", new { id = model.ProductId });
            }

            var review = new Review
            {
                UserId = userId,
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["ReviewSuccess"] = "Your review has been submitted!";
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        // POST: /Review/Delete
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            string userId = _userManager.GetUserId(User);

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }
}