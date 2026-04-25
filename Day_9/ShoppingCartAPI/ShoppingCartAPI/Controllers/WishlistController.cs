using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WishlistItemDto>>> GetWishlist()
        {
            var dtos = await _wishlistService.GetWishlistAsync();
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto dto)
        {
            var resultMessage = await _wishlistService.AddToWishlistAsync(dto);
            return Ok(new { Message = resultMessage });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var resultMessage = await _wishlistService.RemoveFromWishlistAsync(productId);
            return Ok(new { Message = resultMessage });
        }
    }
}
