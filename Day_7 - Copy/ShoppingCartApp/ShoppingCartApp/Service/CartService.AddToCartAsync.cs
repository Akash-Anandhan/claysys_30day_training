// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        public async Task<ServiceResponse> AddToCartAsync(AddToCartDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return ServiceResponse.ShowView("Index", null, string.Empty, "Product not found.");
            var existingItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == dto.UserId && c.ProductId == dto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem { UserId = dto.UserId, ProductId = dto.ProductId, Quantity = dto.Quantity, SellingPrice = product.SellingPrice, BasePrice = product.BasePrice });
            }

            await _context.SaveChangesAsync();
            // AJAX calls get a succeeded response with no redirect —
            // the controller checks IsAjax and returns Json() instead.
            if (dto.IsAjax)
                return new ServiceResponse
                {
                    Succeeded = true
                };
            return ServiceResponse.Redirect("Index", "Cart");
        }
    }
}