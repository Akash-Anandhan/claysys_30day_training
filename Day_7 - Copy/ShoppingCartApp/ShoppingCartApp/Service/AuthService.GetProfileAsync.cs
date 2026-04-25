// Services/AuthService.cs
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResponse.ShowView("NotFound", null, string.Empty, "User not found.");
            return ServiceResponse.ShowView("Profile", new ShoppingCartApp.ViewModels.ProfileViewModel { FullName = user.FullName, Email = user.Email, Address = user.Address });
        }
    }
}