using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/wishlist")]
    [Authorize]
    public class WishlistController : ApiController
    {
        private readonly ShoppingCartAPI.Services.IWishlistService _wishlistService;

        public WishlistController(ShoppingCartAPI.Services.IWishlistService wishlistService) { _wishlistService = wishlistService; }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetWishlist()
        {
            var dtos = await _wishlistService.GetWishlistAsync();
            return Ok(dtos);
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddToWishlist(AddWishlistDto dto)
        {
            var resultMessage = await _wishlistService.AddToWishlistAsync(dto);
            return Ok(new { Message = resultMessage });
        }

        [HttpDelete]
        [Route("{productId:int}")]
        public async Task<IHttpActionResult> RemoveFromWishlist(int productId)
        {
            var resultMessage = await _wishlistService.RemoveFromWishlistAsync(productId);
            return Ok(new { Message = resultMessage });
        }
    }
}



