using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using Microsoft.Extensions.Logging;
using ShoppingCartAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService : IProductsService
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<ProductsService> _logger;
        private readonly IUserContextService _userContextService;
        private readonly IReviewService _reviewService;
        private readonly IInventoryService _inventoryService;
        private readonly IOfferService _offerService;
        private readonly IMemoryCache _cache;

        public ProductsService(
            ShopDbContext context, 
            ILogger<ProductsService> logger, 
            IUserContextService userContextService,
            IReviewService reviewService,
            IInventoryService inventoryService,
            IOfferService offerService,
            IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _userContextService = userContextService;
            _reviewService = reviewService;
            _inventoryService = inventoryService;
            _offerService = offerService;
            _cache = cache;
        }
    }
}
