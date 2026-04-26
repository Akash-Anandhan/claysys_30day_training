// Controllers/WishlistController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Services;

namespace ShoppingCartApp.Controllers
{
    [Authorize]
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET: /Wishlist
        public async Task<IActionResult> Index()
        {
            return Execute(await _wishlistService.GetWishlistAsync(GetUserId()));
        }

        // POST: /Wishlist/Add
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            return Execute(await _wishlistService.AddToWishlistAsync(new AddToWishlistDto
            {
                UserId = GetUserId(),
                ProductId = productId
            }));
        }

        // POST: /Wishlist/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            return Execute(await _wishlistService.RemoveFromWishlistAsync(new RemoveFromWishlistDto
            {
                UserId = GetUserId(),
                ItemId = id
            }));
        }

        // POST: /Wishlist/MoveToCart
        [HttpPost]
        public async Task<IActionResult> MoveToCart(int id)
        {
            return Execute(await _wishlistService.MoveToCartAsync(new MoveToCartDto
            {
                UserId = GetUserId(),
                ItemId = id
            }));
        }

        // GET: /Wishlist/GetWishlistCount (AJAX)
        public async Task<IActionResult> GetWishlistCount()
        {
            var count = await _wishlistService.GetWishlistCountAsync(GetUserId());
            return Json(new { count });
        }

        // GET: /Wishlist/GetWishlistIds (AJAX) - Returns list of product IDs in user's wishlist
        public async Task<IActionResult> GetWishlistIds()
        {
            var result = await _wishlistService.GetWishlistAsync(GetUserId());
            if (result.Succeeded && result.ViewModel != null)
            {
                var wishlist = result.ViewModel as List<ShoppingCartApp.DTOs.Wishlist.WishlistItemDto>;
                var ids = wishlist?.Select(w => w.ProductId).ToList() ?? new List<int>();
                return Json(ids);
            }
            return Json(new List<int>());
        }

        // POST: /Wishlist/RemoveFromWishlist (AJAX) - Removes by productId instead of itemId
        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var result = await _wishlistService.RemoveFromWishlistByProductAsync(GetUserId(), productId);
            return Json(new { success = result.Succeeded, message = result.Succeeded ? "Removed from wishlist" : "Item not found" });
        }

        // POST: /Wishlist/AddToWishlist (AJAX) - Adds product without redirect
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var result = await _wishlistService.AddToWishlistAsync(new AddToWishlistDto
            {
                UserId = GetUserId(),
                ProductId = productId
            });
            // The service returns a redirect, but we want JSON for AJAX
            return Json(new { success = true, message = "Added to wishlist" });
        }
    }
}