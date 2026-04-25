// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ShopDbContext _context;

        public WishlistService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse> GetWishlistAsync(string userId)
        {
            var items = await _context.WishlistItems
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedOn)
                .ToListAsync();

            var wishlistDtos = items.Select(w => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                ImageUrl = w.Product.ImageUrl,
                Price = w.Product.Price,
                CategoryName = w.Product.Category?.Name,
                AddedOn = w.AddedOn
            }).ToList();

            return ServiceResponse.ShowView("Index", wishlistDtos);
        }

        public async Task<ServiceResponse> AddToWishlistAsync(AddToWishlistDto dto)
        {
            var alreadyExists = await _context.WishlistItems
                .AnyAsync(w => w.UserId == dto.UserId && w.ProductId == dto.ProductId);

            if (!alreadyExists)
            {
                _context.WishlistItems.Add(new WishlistItem
                {
                    UserId = dto.UserId,
                    ProductId = dto.ProductId,
                    AddedOn = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Index", "Wishlist");
        }

        public async Task<ServiceResponse> RemoveFromWishlistAsync(RemoveFromWishlistDto dto)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == dto.ItemId && w.UserId == dto.UserId);

            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return ServiceResponse.Redirect("Index", "Wishlist");
        }

        public async Task<ServiceResponse> MoveToCartAsync(MoveToCartDto dto)
        {
            var wishlistItem = await _context.WishlistItems
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.Id == dto.ItemId && w.UserId == dto.UserId);

            if (wishlistItem == null)
                return ServiceResponse.ShowView(
                    "NotFound", null, string.Empty, "Wishlist item not found.");

            // Add to cart — increment if already exists
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId
                                       && c.ProductId == wishlistItem.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = dto.UserId,
                    ProductId = wishlistItem.ProductId,
                    Quantity = 1,
                    UnitPrice = wishlistItem.Product.Price
                });
            }

            // Remove from wishlist now that it's in the cart
            _context.WishlistItems.Remove(wishlistItem);

            await _context.SaveChangesAsync();

            return ServiceResponse.Redirect("Index", "Cart");
        }

        public async Task<int> GetWishlistCountAsync(string userId)
        {
            return await _context.WishlistItems
                .CountAsync(w => w.UserId == userId);
        }
    }
}