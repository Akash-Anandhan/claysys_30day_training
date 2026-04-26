// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class CartService
    {
        public async Task<UpdateQuantityResultDto> UpdateQuantityAsync(UpdateQuantityDto dto)
        {
            var item = await _context.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == dto.ItemId && c.UserId == dto.UserId);
            if (item == null)
                return new UpdateQuantityResultDto
                {
                    Success = false,
                    Message = "Item not found."
                };
            if (dto.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return new UpdateQuantityResultDto
                {
                    Success = true,
                    Removed = true,
                    CartTotal = await CalculateCartTotalAsync(dto.UserId)
                };
            }

            item.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();
            return new UpdateQuantityResultDto
            {
                Success = true,
                Removed = false,
                Subtotal = (item.SellingPrice * item.Quantity).ToString("0.00"),
                CartTotal = await CalculateCartTotalAsync(dto.UserId),
                CartCount = await CalculateCartCountAsync(dto.UserId)
            };
        }
    }
}