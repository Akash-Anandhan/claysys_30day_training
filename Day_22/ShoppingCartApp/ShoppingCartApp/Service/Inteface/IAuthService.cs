// Services/IAuthService.cs
using ShoppingCartApp.DTOs.Auth;

namespace ShoppingCartApp.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse> RegisterAsync(RegisterDto dto);
        Task<ServiceResponse> LoginAsync(LoginDto dto);
        Task<ServiceResponse> LogoutAsync();
        Task<ServiceResponse> GetProfileAsync(string userId);
        Task<ServiceResponse> UpdateProfileAsync(UpdateProfileDto dto, ShoppingCartApp.ViewModels.ProfileViewModel model = null);
        Task<ServiceResponse> GeneratePasswordResetTokenAsync(ForgotPasswordDto dto);
        Task<ServiceResponse> ResetPasswordAsync(ResetPasswordDto dto);
    }
}