using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace ShoppingCartAPI.Services
{
    public partial class AuthService
    {
        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid password");

            var token = await GetToken(user);
            var refreshToken = GenerateRefreshToken();

            _ = int.TryParse(System.Configuration.ConfigurationManager.AppSettings["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
            if (refreshTokenValidityInDays == 0) refreshTokenValidityInDays = 7;

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Email = user.Email,
                FullName = user.FullName,
                Expiration = token.ValidTo.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}

