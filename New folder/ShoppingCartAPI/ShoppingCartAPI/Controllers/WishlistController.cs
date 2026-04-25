using ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;


namespace ShoppingCartAPI.Controllers
{
    [RoutePrefix("api/wishlist")]
    [Authorize]
    public class WishlistController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet, Route("")]
        public IHttpActionResult Get()
        {
            var userId = User.Identity.GetUserId();
            return Ok(_db.WishlistItems.Include("Product").Where(w => w.UserId == userId).ToList());
        }

        [HttpPost, Route("add/{productId}")]
        public IHttpActionResult Add(int productId)
        {
            var userId = User.Identity.GetUserId();
            if (_db.WishlistItems.Any(w => w.UserId == userId && w.ProductId == productId))
                return BadRequest("Already in wishlist");
            _db.WishlistItems.Add(new WishlistItem { UserId = userId, ProductId = productId });
            _db.SaveChanges();
            return Ok("Added to wishlist");
        }

        [HttpDelete, Route("{id}")]
        public IHttpActionResult Remove(int id)
        {
            var item = _db.WishlistItems.Find(id);
            if (item == null || item.UserId != User.Identity.GetUserId()) return NotFound();
            _db.WishlistItems.Remove(item);
            _db.SaveChanges();
            return Ok("Removed");
        }
    }
}