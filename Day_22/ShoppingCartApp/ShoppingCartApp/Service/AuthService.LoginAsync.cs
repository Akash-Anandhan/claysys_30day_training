// Services/AuthService.cs
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> LoginAsync(LoginDto dto)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
                return ServiceResponse.ShowView("Login", new LoginDto { Email = dto.Email, RememberMe = dto.RememberMe }, string.Empty, "Invalid email or password.");
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (!string.IsNullOrEmpty(dto.GuestId))
                await _cartService.MergeGuestCartAsync(dto.GuestId, user.Id);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Index",
                RedirectController = isAdmin ? "Admin" : "Home",
                SessionRemoveKey = "GuestId"
            };
        }
    }
}