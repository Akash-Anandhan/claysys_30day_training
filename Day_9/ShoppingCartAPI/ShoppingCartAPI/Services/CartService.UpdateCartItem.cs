using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Services
{
    public partial class CartService
    {
        public async Task<object> UpdateCartItemAsync(int id, UpdateCartDto dto)
        {
            try
            {
                var userId = _userContextService.GetUserId();
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

                if (cartItem == null)
                    throw new KeyNotFoundException($"Item with ID {id} not found in cart.");

                var product = await GetProductInfoAsync(cartItem.ProductId);

                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {cartItem.ProductId} not found.");

                if (product.Stock < dto.Quantity)
                    throw new ArgumentException($"Not enough stock. Requested: {dto.Quantity}, Available: {product.Stock}");

                cartItem.Quantity = dto.Quantity;

                await _context.SaveChangesAsync();

                return new
                {
                    Message = "Cart item updated successfully.",
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Item or product not found during cart update. ItemId: {ItemId}", id);
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed during cart update. ItemId: {ItemId}, Quantity: {Quantity}", id, dto.Quantity);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating cart item. ItemId: {ItemId}", id);
                throw;
            }
        }
    }
}
