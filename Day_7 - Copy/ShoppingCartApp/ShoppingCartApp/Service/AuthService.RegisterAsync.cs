// Services/AuthService.cs
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return ServiceResponse.ShowView("Register", new RegisterDto { FullName = dto.FullName, Email = dto.Email }, result.Errors.ToDictionary(_ => string.Empty, e => e.Description));
            await _signInManager.SignInAsync(user, isPersistent: false);
            return ServiceResponse.Redirect("Index", "Home");
        }
    }
}