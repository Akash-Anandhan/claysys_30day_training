// Services/CartService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Cart;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public class CartService : ICartService
    {
        private readonly ShopDbContext _context;

        public CartService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse> GetCartAsync(string userId)
        {
            var items = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var cartDto = new CartDto
            {
                Items = items.Select(c => new CartItemDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ImageUrl = c.Product.ImageUrl,
                    UnitPrice = c.UnitPrice,
                    Quantity = c.Quantity,
                    Subtotal = c.UnitPrice * c.Quantity
                }).ToList(),
                Total = items.Sum(c => c.UnitPrice * c.Quantity),
                TotalCount = items.Sum(c => c.Quantity)
            };

            return ServiceResponse.ShowView("Index", cartDto);
        }

        public async Task<ServiceResponse> AddToCartAsync(AddToCartDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null)
                return ServiceResponse.ShowView(
                    "Index", null, string.Empty, "Product not found.");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId
                                       && c.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = dto.UserId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }

            await _context.SaveChangesAsync();

            // AJAX calls get a succeeded response with no redirect —
            // the controller checks IsAjax and returns Json() instead.
            if (dto.IsAjax)
                return new ServiceResponse { Succeeded = true };

            return ServiceResponse.Redirect("Index", "Cart");
        }

        public async Task<ServiceResponse> RemoveFromCartAsync(RemoveFromCartDto dto)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == dto.ItemId
                                       && c.UserId == dto.UserId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Index", "Cart");
        }

        public async Task<UpdateQuantityResultDto> UpdateQuantityAsync(UpdateQuantityDto dto)
        {
            var item = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == dto.ItemId
                                       && c.UserId == dto.UserId);

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
                Subtotal = (item.UnitPrice * item.Quantity).ToString("0.00"),
                CartTotal = await CalculateCartTotalAsync(dto.UserId),
                CartCount = await CalculateCartCountAsync(dto.UserId)
            };
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await CalculateCartCountAsync(userId);
        }

        public async Task MergeGuestCartAsync(string guestId, string userId)
        {
            var guestItems = await _context.CartItems
                .Where(c => c.UserId == guestId)
                .ToListAsync();

            foreach (var guestItem in guestItems)
            {
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId
                                           && c.ProductId == guestItem.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += guestItem.Quantity;
                    _context.CartItems.Remove(guestItem);
                }
                else
                {
                    guestItem.UserId = userId;
                }
            }

            await _context.SaveChangesAsync();
        }

        // ── Private helpers ────────────────────────────────────────────────
        private async Task<string> CalculateCartTotalAsync(string userId)
        {
            var total = await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.UnitPrice * c.Quantity);

            return total.ToString("0.00");
        }

        private async Task<int> CalculateCartCountAsync(string userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }
    }
}