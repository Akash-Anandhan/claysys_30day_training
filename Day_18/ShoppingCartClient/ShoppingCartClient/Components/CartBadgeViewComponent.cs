using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartClient.Components
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly IApiService _apiService;

        public CartBadgeViewComponent(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var cart = await _apiService.GetCartAsync();
                int count = cart?.Items?.Sum(i => i.Quantity) ?? 0;
                return View("Default", count);
            }
            return View("Default", 0);
        }
    }
}
