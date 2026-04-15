using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemResponseDto>>> GetCart()
        {
            string userId = GetUserId();
            var dtos = await _cartService.GetCartAsync(userId);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            string userId = GetUserId();

            try
            {
                var resultMessage = await _cartService.AddToCartAsync(userId, dto);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            string userId = GetUserId();

            try
            {
                var resultMessage = await _cartService.RemoveFromCartAsync(userId, id);
                return Ok(new { Message = resultMessage });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartDto dto)
        {
            string userId = GetUserId();

            try
            {
                var result = await _cartService.UpdateCartItemAsync(userId, id, dto);
                return Ok(result);
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

        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return Ok(new
            {
                IsAuth = User.Identity?.IsAuthenticated,
                UserId = GetUserId(),
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}