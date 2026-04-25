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
        public async Task<AuthResponseDto> RefreshAsync(TokenApiDto tokenApiDto)
        {

            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                throw new ArgumentException("Invalid access token or refresh token");

            string username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new ArgumentException("Invalid access token or refresh token");

            var newAccessToken = await GetToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
                Email = user.Email,
                FullName = user.FullName,
                Expiration = newAccessToken.ValidTo.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}

