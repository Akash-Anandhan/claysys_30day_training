using ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ShoppingCartAPI.Controllers
{
    // Controllers/ProductsController.cs
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet, Route("")]
        [AllowAnonymous]  // anyone can browse products
        public IHttpActionResult GetAll() => Ok(_db.Products.ToList());

        [HttpGet, Route("{id}")]
        [AllowAnonymous]
        public IHttpActionResult Get(int id)
        {
            var p = _db.Products.Find(id);
            return p == null ? (IHttpActionResult)NotFound() : Ok(p);
        }

        [HttpPost, Route("")]
        [Authorize(Roles = "Admin")]   // only admins can create
        public IHttpActionResult Create(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
            return Created($"api/products/{product.Id}", product);
        }

        [HttpPut, Route("{id}")]
        [Authorize(Roles = "Admin")]   // only admins can edit
        public IHttpActionResult Update(int id, Product updated)
        {
            var p = _db.Products.Find(id);
            if (p == null) return NotFound();
            p.Name = updated.Name; p.Price = updated.Price;
            p.Description = updated.Description; p.Stock = updated.Stock;
            _db.SaveChanges();
            return Ok(p);
        }

        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Admin")]   // only admins can delete
        public IHttpActionResult Delete(int id)
        {
            var p = _db.Products.Find(id);
            if (p == null) return NotFound();
            _db.Products.Remove(p);
            _db.SaveChanges();
            return Ok("Deleted");
        }
    }
}