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
    public partial class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShopDbContext _context;
        private readonly IConfiguration _configuration;
        public TokenService(UserManager<ApplicationUser> userManager, ShopDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }
    }
}