// Services/AuthService.cs
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public partial class AuthService
    {
        public async Task<ServiceResponse> GeneratePasswordResetTokenAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            // Always redirect — never reveal whether the email exists
            if (user == null)
                return ServiceResponse.Redirect("ForgotPasswordConfirmation", "Account");
            return ServiceResponse.Redirect("ForgotPasswordConfirmation", "Account", new Dictionary<string, string> { { "ResetLink", dto.ResetUrl } });
        }
    }
}