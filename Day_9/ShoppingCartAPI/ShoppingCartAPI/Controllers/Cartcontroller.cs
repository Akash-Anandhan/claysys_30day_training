using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var cartDto = await _cartService.GetCartAsync();
            return Ok(cartDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var resultMessage = await _cartService.AddToCartAsync(dto);
            return Ok(new { Message = resultMessage });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var resultMessage = await _cartService.RemoveFromCartAsync(id);
            return Ok(new { Message = resultMessage });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartDto dto)
        {
            var result = await _cartService.UpdateCartItemAsync(id, dto);
            return Ok(result);
        }

       
    }
}