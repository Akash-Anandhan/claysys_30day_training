using System.Linq;
using System.Data.Entity;
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
        public async Task<string> RemoveFromCartAsync(int id)
        {
            try
            {
                var userId = _userContextService.GetUserId();
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

                if (cartItem == null)
                    throw new KeyNotFoundException($"Item with ID {id} not found in cart.");

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return "Item removed from cart.";
            }
            catch (KeyNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}


