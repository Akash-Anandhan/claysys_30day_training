using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using System.Security.Claims;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public CartController(ShopDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemResponseDto>>> GetCart()
        {
            string userId = GetUserId();
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var dtos = cartItems.Select(c => new CartItemResponseDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product?.Name,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice,
                TotalPrice = c.Quantity * c.UnitPrice
            });

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            string userId = GetUserId();
            var product = await _context.Products.FindAsync(dto.ProductId);
            
            if (product == null)
                return NotFound(new { Message = "Product not found." });

            if (product.Stock < dto.Quantity)
                return BadRequest(new { Message = "Not enough stock." });

            var existingItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Item added to cart successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            string userId = GetUserId();
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            
            if (cartItem == null)
                return NotFound(new { Message = "Item not found in cart." });

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Item removed from cart." });
        }
    }
}
