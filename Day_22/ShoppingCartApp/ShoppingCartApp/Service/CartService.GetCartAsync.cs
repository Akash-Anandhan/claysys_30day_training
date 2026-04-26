// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        public async Task<ServiceResponse> GetCartAsync(string userId)
        {
            var items = await _context.CartItems.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();
            var cartDto = new CartDto
            {
                Items = items.Select(c => new CartItemDto { Id = c.Id, ProductId = c.ProductId, ProductName = c.Product.Name, ImageUrl = c.Product.ImageUrl, UnitPrice = c.SellingPrice, Quantity = c.Quantity, Subtotal = c.SellingPrice * c.Quantity }).ToList(),
                Total = items.Sum(c => c.SellingPrice * c.Quantity),
                TotalCount = items.Sum(c => c.Quantity)
            };
            return ServiceResponse.ShowView("Index", cartDto);
        }
    }
}