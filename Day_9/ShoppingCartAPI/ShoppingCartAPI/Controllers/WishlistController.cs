using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;
using System.Security.Claims;

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

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WishlistItemDto>>> GetWishlist()
        {
            var userId = GetUserId();
            var dtos = await _wishlistService.GetWishlistAsync(userId);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto dto)
        {
            var userId = GetUserId();

            try
            {
                var resultMessage = await _wishlistService.AddToWishlistAsync(userId, dto);
                return Ok(new { Message = resultMessage });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = GetUserId();

            try
            {
                var resultMessage = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
                return Ok(new { Message = resultMessage });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
