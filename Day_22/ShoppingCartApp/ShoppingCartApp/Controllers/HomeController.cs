using Microsoft.AspNetCore.Mvc;
using ShoppingCartApp.Controllers.Base;
using ShoppingCartApp.Services;

namespace ShoppingCartApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _homeService.GetHomeViewModelAsync();
            return View(viewModel);
        }
    }
}