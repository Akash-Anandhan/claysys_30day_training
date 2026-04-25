// Services/TokenService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoppingCartApp.Services
{
    public partial class TokenService
    {
        public async Task<TokenResultDto> GenerateTokenAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return null;
            return await BuildTokenPairAsync(user);
        }
    }
}