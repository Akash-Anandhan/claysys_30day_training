using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto model);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
        Task<AuthResponseDto> RefreshAsync(TokenApiDto tokenApiDto);
        Task<UserProfileDto> ViewProfileAsync();
    }
}

