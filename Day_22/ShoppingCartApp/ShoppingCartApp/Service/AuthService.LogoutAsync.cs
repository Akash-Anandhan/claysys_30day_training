// Services/AuthService.cs
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Index",
                RedirectController = "Home",
                SessionRemoveKey = "__ALL__"
            };
        }
    }
}