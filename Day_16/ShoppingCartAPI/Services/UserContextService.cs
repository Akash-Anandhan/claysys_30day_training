using System.Web;
using Microsoft.AspNet.Identity;
using ShoppingCartAPI.Services.Interfaces;

namespace ShoppingCartAPI.Services
{
    public class UserContextService : IUserContextService
    {
        public string GetUserId()
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }
            return null;
        }
    }
}
