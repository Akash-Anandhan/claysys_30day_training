using System.Data.Entity;
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
        public async Task<string> AddToCartAsync(AddToCartDto dto)
        {
            try
            {
                var userId = _userContextService.GetUserId();
                var product = await _context.Products.FindAsync(dto.ProductId);

                if (product == null)
                    throw new KeyNotFoundException("Product not found.");

                if (product.Stock < dto.Quantity)
                    throw new ArgumentException("Not enough stock.");

                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

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

                return "Item added to cart successfully.";
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Product not found when adding to cart. ProductId: {ProductId}", dto.ProductId);
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed: not enough stock. ProductId: {ProductId}, Quantity: {Quantity}", dto.ProductId, dto.Quantity);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding to cart. ProductId: {ProductId}", dto.ProductId);
                throw;
            }
        }
    }
}
