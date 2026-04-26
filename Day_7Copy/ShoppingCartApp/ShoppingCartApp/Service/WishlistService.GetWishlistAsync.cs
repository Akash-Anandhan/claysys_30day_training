// Services/WishlistService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Wishlist;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class WishlistService
    {
        public async Task<ServiceResponse> GetWishlistAsync(string userId)
        {
            var items = await _context.WishlistItems.Include(w => w.Product).ThenInclude(p => p.Category).Where(w => w.UserId == userId).OrderByDescending(w => w.AddedOn).ToListAsync();
            var wishlistDtos = items.Select(w => new WishlistItemDto { Id = w.Id, ProductId = w.ProductId, ProductName = w.Product.Name, ImageUrl = w.Product.ImageUrl, Price = w.Product.SellingPrice, CategoryName = w.Product.Category?.Name, AddedOn = w.AddedOn }).ToList();
            return ServiceResponse.ShowView("Index", wishlistDtos);
        }
    }
}