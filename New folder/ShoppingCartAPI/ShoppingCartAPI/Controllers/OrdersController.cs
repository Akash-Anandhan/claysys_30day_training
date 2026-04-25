using ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/orders")]
    [Authorize]
    public class OrdersController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet, Route("")]
        public IHttpActionResult GetMyOrders()
        {
            var userId = User.Identity.GetUserId();
            return Ok(_db.Orders.Include("OrderItems.Product").Where(o => o.UserId == userId).ToList());
        }

        [HttpPost, Route("place")]
        public IHttpActionResult PlaceOrder()
        {
            var userId = User.Identity.GetUserId();
            var cartItems = _db.CartItems.Include("Product").Where(c => c.UserId == userId).ToList();
            if (!cartItems.Any()) return BadRequest("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.Price
                }).ToList()
            };
            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems); // clear cart after order
            _db.SaveChanges();
            return Ok(order);
        }
    }
}