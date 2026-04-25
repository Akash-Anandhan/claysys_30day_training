// Services/ProductService.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class ProductService : IProductService
    {
        private readonly ShopDbContext _context;
        public ProductService(ShopDbContext context)
        {
            _context = context;
        }
    }
}