using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/cart")]
    public class CartController : ApiController
    {
        private readonly ShoppingCartAPI.Services.ICartService _cartService;

        public CartController(ShoppingCartAPI.Services.ICartService cartService) { _cartService = cartService; }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetCart()
        {
            var cartDto = await _cartService.GetCartAsync();
            return Ok(cartDto);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddToCart(AddToCartDto dto)
        {
            var resultMessage = await _cartService.AddToCartAsync(dto);
            return Ok(new { Message = resultMessage });
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> RemoveFromCart(int id)
        {
            var resultMessage = await _cartService.RemoveFromCartAsync(id);
            return Ok(new { Message = resultMessage });
        }

        [HttpPut]
        [Route("update/{id:int}")]
        public async Task<IHttpActionResult> UpdateCartItem(int id, UpdateCartDto dto)
        {
            var result = await _cartService.UpdateCartItemAsync(id, dto);
            return Ok(result);
        }
    }
}



