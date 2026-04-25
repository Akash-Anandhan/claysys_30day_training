using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services.Interfaces;
using System.Runtime.Caching;

namespace ShoppingCartAPI.Services
{
    public partial class ProductsService : IProductsService
    {
        private readonly ShopDbContext _context;

        private readonly IUserContextService _userContextService;
        private readonly IReviewService _reviewService;
        private readonly IInventoryService _inventoryService;
        private readonly IOfferService _offerService;
        private readonly MemoryCache _cache = MemoryCache.Default;

        public ProductsService(
            ShopDbContext context, 
            IUserContextService userContextService,
            IReviewService reviewService,
            IInventoryService inventoryService,
            IOfferService offerService)
        {
            _context = context;

            _userContextService = userContextService;
            _reviewService = reviewService;
            _inventoryService = inventoryService;
            _offerService = offerService;

        }
    }
}

