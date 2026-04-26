// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;
using System.Globalization;

namespace ShoppingCartApp.Services
{
    public partial class ProductService : IProductService
    {
        private readonly ShopDbContext _context;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ShortCacheDuration = TimeSpan.FromMinutes(5);

        public ProductService(ShopDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
    }
}