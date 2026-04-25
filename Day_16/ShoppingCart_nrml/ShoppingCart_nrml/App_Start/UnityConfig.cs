using System.Web.Http;
using Unity;
using Unity.WebApi;
using ShoppingCartAPI.Services;
using ShoppingCartAPI.Services.Interfaces;
using ShoppingCartAPI.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ShoppingCartAPI.Models;

namespace ShoppingCart_nrml
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // Register all your components with the container here
            container.RegisterType<ShopDbContext>();
            
            // Identity framework types
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new Unity.Injection.InjectionConstructor(new ShopDbContext()));
            container.RegisterType<UserManager<ApplicationUser>>();
            container.RegisterType<IRoleStore<IdentityRole, string>, RoleStore<IdentityRole>>(new Unity.Injection.InjectionConstructor(new ShopDbContext()));
            container.RegisterType<RoleManager<IdentityRole>>();

            // Custom Services
            container.RegisterType<IUserContextService, UserContextService>();
            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IProductsService, ProductsService>();
            container.RegisterType<IInventoryService, InventoryService>();
            container.RegisterType<IOfferService, OfferService>();
            container.RegisterType<IReviewService, ReviewService>();
            container.RegisterType<ICartService, CartService>();
            container.RegisterType<IOrdersService, OrdersService>();
            container.RegisterType<IWishlistService, WishlistService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
