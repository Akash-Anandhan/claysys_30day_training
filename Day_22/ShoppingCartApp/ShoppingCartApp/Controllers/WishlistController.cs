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
            var response = await _wishlistService.GetWishlistAsync(GetUserId());
            return ExecuteServiceResponse(response);
        }

        // POST: /Wishlist/Add
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var dto = new AddToWishlistDto { UserId = GetUserId(), ProductId = productId };
            return ExecuteServiceResponse(await _wishlistService.AddToWishlistAsync(dto));
        }

        // POST: /Wishlist/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var dto = new RemoveFromWishlistDto { UserId = GetUserId(), ItemId = id };
            return ExecuteServiceResponse(await _wishlistService.RemoveFromWishlistAsync(dto));
        }

        // POST: /Wishlist/MoveToCart
        [HttpPost]
        public async Task<IActionResult> MoveToCart(int id)
        {
            var dto = new MoveToCartDto { UserId = GetUserId(), ItemId = id };
            return ExecuteServiceResponse(await _wishlistService.MoveToCartAsync(dto));
        }

        // GET: /Wishlist/GetWishlistCount (AJAX)
        public async Task<IActionResult> GetWishlistCount()
        {
            var count = await _wishlistService.GetWishlistCountAsync(GetUserId());
            return Json(new { count });
        }

        // GET: /Wishlist/GetWishlistIds (AJAX)
        public async Task<IActionResult> GetWishlistIds()
        {
            var result = await _wishlistService.GetWishlistAsync(GetUserId());
            if (result.Succeeded && result.ViewModel != null)
            {
                var wishlist = result.ViewModel as List<WishlistItemDto>;
                var ids = wishlist?.Select(w => w.ProductId).ToList() ?? new List<int>();
                return Json(ids);
            }
            return Json(new List<int>());
        }

        // POST: /Wishlist/RemoveFromWishlist (AJAX)
        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var result = await _wishlistService.RemoveFromWishlistByProductAsync(GetUserId(), productId);
            var message = result.Succeeded ? WishlistService.MessageRemoved : WishlistService.MessageNotFound;
            return Json(new { success = result.Succeeded, message });
        }

        // POST: /Wishlist/AddToWishlist (AJAX)
        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var result = await _wishlistService.AddToWishlistAsync(new AddToWishlistDto
            {
                UserId = GetUserId(),
                ProductId = productId
            });
            return Json(new { success = true, message = WishlistService.MessageAdded });
        }
    }
}