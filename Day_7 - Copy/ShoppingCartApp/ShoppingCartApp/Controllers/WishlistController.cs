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
    }
}