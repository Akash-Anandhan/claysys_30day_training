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
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShopDbContext _context;
        private readonly IConfiguration _configuration;

        public TokenService(
            UserManager<ApplicationUser> userManager,
            ShopDbContext context,
            IConfiguration configuration)
        {
            _userManager   = userManager;
            _context       = context;
            _configuration = configuration;
        }

        public async Task<TokenResultDto> GenerateTokenAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return null;

            return await BuildTokenPairAsync(user);
        }

        public async Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key         = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            var handler     = new JwtSecurityTokenHandler();

            try
            {
                // Validate signature and format — intentionally skip lifetime check
                var principal = handler.ValidateToken(dto.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidIssuer              = jwtSettings["Issuer"],
                    ValidAudience            = jwtSettings["Audience"],
                    ValidateLifetime         = false
                }, out var validatedToken);

                // Ensure HMAC-SHA256 was used
                if (validatedToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    return null;

                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);

                if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked)
                    return null;

                var jti = principal.Claims
                    .SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

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

        // ── Private helpers ────────────────────────────────────────────────────

        private async Task<TokenResultDto> BuildTokenPairAsync(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key         = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,   user.Id),
                new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier,     user.Id),
                new(ClaimTypes.Name,               user.UserName)
            };

            claims.AddRange(await _userManager.GetClaimsAsync(user));

            foreach (var role in await _userManager.GetRolesAsync(user))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var descriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Expires            = DateTime.UtcNow.AddMinutes(10),
                Issuer             = jwtSettings["Issuer"],
                Audience           = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var handler      = new JwtSecurityTokenHandler();
            var jwtToken     = handler.CreateToken(descriptor);
            var accessToken  = handler.WriteToken(jwtToken);

            var refreshToken = new RefreshToken
            {
                JwtId      = jwtToken.Id,
                IsUsed     = false,
                IsRevoked  = false,
                UserId     = user.Id,
                AddedDate  = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
                Token      = $"{Guid.NewGuid()}-{Guid.NewGuid()}"
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new TokenResultDto
            {
                Token        = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
