using Microsoft.AspNetCore.Mvc;
using ShoppingCartClient.Services;
using System.Threading.Tasks;

namespace ShoppingCartClient.Components
{
    public class WishlistBadgeViewComponent : ViewComponent
    {
        private readonly IApiService _apiService;

        public WishlistBadgeViewComponent(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var wishlist = await _apiService.GetWishlistAsync();
                int count = wishlist?.Count ?? 0;
                return View("Default", count);
            }
            return View("Default", 0);
        }
    }
}
