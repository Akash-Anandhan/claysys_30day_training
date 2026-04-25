using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/orders")]
    [Authorize]
    public class OrdersController : ApiController
    {
        private readonly ShoppingCartAPI.Services.IOrdersService _ordersService;

        public OrdersController(ShoppingCartAPI.Services.IOrdersService ordersService) { _ordersService = ordersService; }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetOrders()
        {
            var dtos = await _ordersService.GetOrdersAsync();
            return Ok(dtos);
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<IHttpActionResult> Checkout(CheckoutDto dto)
        {
            var result = await _ordersService.CheckoutAsync(dto);
            return Ok(result);
        }
    }
}



