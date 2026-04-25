// Services/AuthService.cs
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ServiceResponse.Redirect("ResetPasswordConfirmation", "Account");
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.Password);
            if (!result.Succeeded)
                return ServiceResponse.ShowView("ResetPassword", new ResetPasswordDto { Email = dto.Email, Token = dto.Token }, result.Errors.ToDictionary(_ => string.Empty, e => e.Description));
            return ServiceResponse.Redirect("ResetPasswordConfirmation", "Account");
        }
    }
}