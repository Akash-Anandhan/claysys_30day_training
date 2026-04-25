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
        public async Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            var handler = new JwtSecurityTokenHandler();
            try
            {
                // Validate signature and format — intentionally skip lifetime check
                var principal = handler.ValidateToken(dto.Token, new TokenValidationParameters { ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(key), ValidateIssuer = true, ValidateAudience = true, ValidIssuer = jwtSettings["Issuer"], ValidAudience = jwtSettings["Audience"], ValidateLifetime = false }, out var validatedToken);
                // Ensure HMAC-SHA256 was used
                if (validatedToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);
                if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked)
                    return null;
                var jti = principal.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (storedToken.JwtId != jti || storedToken.ExpiryDate < DateTime.UtcNow)
                    return null;
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();
                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                return await BuildTokenPairAsync(user);
            }
            catch
            {
                return null;
            }
        }
    }
}