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
        private readonly OrdersService _ordersService;

        public OrdersController()
        {
            _ordersService = new OrdersService();
        }

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
