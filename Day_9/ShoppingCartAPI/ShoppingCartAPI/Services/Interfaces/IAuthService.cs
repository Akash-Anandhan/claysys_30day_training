using ShoppingCartAPI.DTOs;

namespace ShoppingCartAPI.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto model);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
        Task<AuthResponseDto> RefreshAsync(TokenApiDto tokenApiDto);
        Task<UserProfileDto> ViewProfileAsync();
        Task<IEnumerable<UserProfileDto>> GetUsersAsync();
    }
}
