using System.Data.Entity;
using Microsoft.Extensions.Logging;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Services
{
    public partial class CartService
    {
        public async Task<CartDto> GetCartAsync()
        {
            try
            {
                var userId = _userContextService.GetUserId();
                
                // 1. Fetch cart from repository sequentially
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _logger.LogInformation("Start of GetCart for user {UserId}. Number of items: {Count}", userId, cartItems.Count);

                // 2. Prepare parallel tasks
                var productTasks = cartItems.Select(async c => 
                {
                    try 
                    {
                        var product = await _productsService.GetProductAsync(c.ProductId);
                        return new CartItemResponseDto
                        {
                            Id = c.Id,
                            ProductId = c.ProductId,
                            ProductName = product?.Name,
                            Quantity = c.Quantity,
                            UnitPrice = product?.Price ?? c.UnitPrice,
                            TotalPrice = c.Quantity * (product?.Price ?? c.UnitPrice)
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to fetch product details for product ID {ProductId}", c.ProductId);
                        return new CartItemResponseDto
                        {
                            Id = c.Id,
                            ProductId = c.ProductId,
                            ProductName = "Unknown Product",
                            Quantity = c.Quantity,
                            UnitPrice = c.UnitPrice,
                            TotalPrice = c.Quantity * c.UnitPrice
                        };
                    }
                }).ToList();

                var offersTask = _offerService.GetOffersAsync();

                // 3. Await all parallel tasks together
                var allTasks = new List<Task>(productTasks);
                allTasks.Add(offersTask);
                
                await Task.WhenAll(allTasks);

                var finalCartItems = (await Task.WhenAll(productTasks)).ToList();
                var offers = await offersTask;

                return new CartDto
                {
                    Items = finalCartItems,
                    Offers = offers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An expected error occurred while fetching the user's cart.");
                throw;
            }
        }
    }
}
