using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Models;
using ShoppingCartClient.Services;
using System.Threading.Tasks;

namespace ShoppingCartClient.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IApiService _apiService;

        public WishlistController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var wishlist = await _apiService.GetWishlistAsync();
            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var dto = new AddWishlistDto { ProductId = productId };
            await _apiService.AddToWishlistAsync(dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            await _apiService.RemoveFromWishlistAsync(productId);
            return RedirectToAction("Index");
        }
    }
}
