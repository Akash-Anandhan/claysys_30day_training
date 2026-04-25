using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;

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

        private readonly IProductsService _productsService;
        private readonly IOfferService _offerService;

        public CartService(ShopDbContext context, IUserContextService userContextService, IProductsService productsService, IOfferService offerService)
        {
            _context = context;
            _userContextService = userContextService;

            _productsService = productsService;
            _offerService = offerService;
        }
    }
}

