using ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/cart")]
    [Authorize]
    public class CartController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            var userId = User.Identity.GetUserId();
            var items = _db.CartItems.Include("Product").Where(c => c.UserId == userId).ToList();
            return Ok(items);
        }

        [HttpPost, Route("add")]
        public IHttpActionResult Add(CartItemDto dto)
        {
            var userId = User.Identity.GetUserId();
            var existing = _db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == dto.ProductId);
            if (existing != null) existing.Quantity += dto.Quantity;
            else _db.CartItems.Add(new CartItem { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity });
            _db.SaveChanges();
            return Ok("Cart updated");
        }

        [HttpDelete, Route("{id}")]
        public IHttpActionResult Remove(int id)
        {
            var item = _db.CartItems.Find(id);
            if (item == null || item.UserId != User.Identity.GetUserId()) return NotFound();
            _db.CartItems.Remove(item);
            _db.SaveChanges();
            return Ok("Removed");
        }
    }
    public class CartItemDto { public int ProductId { get; set; } public int Quantity { get; set; } }
}
