// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.DTOs.Product;
using ShoppingCartApp.Services;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.Controllers
{
    // Part 1 – main product browsing actions
    public partial class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            return Execute(await _productService.GetAllAsync());
        }

        // GET: /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await _productService.GetDetailsAsync(id);
            if (response.ViewModel == null)
                return NotFound();

            return Execute(response);
        }
    }
}
