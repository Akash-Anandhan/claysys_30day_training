using System.Data.Entity;
using Microsoft.Extensions.Logging;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.DTOs;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services.Interfaces;

namespace ShoppingCartAPI.Services
{
    public partial class CartService : ICartService
    {
        private readonly ShopDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<CartService> _logger;
        private readonly IProductsService _productsService;
        private readonly IOfferService _offerService;

        public CartService(ShopDbContext context, IUserContextService userContextService, ILogger<CartService> logger, IProductsService productsService, IOfferService offerService)
        {
            _context = context;
            _userContextService = userContextService;
            _logger = logger;
            _productsService = productsService;
            _offerService = offerService;
        }
    }
}
