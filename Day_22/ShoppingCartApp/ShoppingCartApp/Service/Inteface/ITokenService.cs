// Services/Interface/ITokenService.cs
using ShoppingCartApp.DTOs.Auth;

namespace ShoppingCartApp.Services
{
    public interface ITokenService
    {
        Task<TokenResultDto> GenerateTokenAsync(string email, string password);
        Task<TokenResultDto> RefreshTokenAsync(RefreshTokenDto dto);
    }
}
