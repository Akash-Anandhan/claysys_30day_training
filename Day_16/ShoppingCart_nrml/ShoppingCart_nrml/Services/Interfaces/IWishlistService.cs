using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemDto>> GetWishlistAsync();
        Task<string> AddToWishlistAsync(AddWishlistDto dto);
        Task<string> RemoveFromWishlistAsync(int productId);
    }
}

