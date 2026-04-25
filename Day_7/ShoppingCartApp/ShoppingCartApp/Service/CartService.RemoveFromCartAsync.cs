// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        public async Task<ServiceResponse> RemoveFromCartAsync(RemoveFromCartDto dto)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == dto.ItemId && c.UserId == dto.UserId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Index", "Cart");
        }
    }
}