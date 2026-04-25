// Services/AuthService.cs
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ShoppingCartApp.DTOs.Auth;
using ShoppingCartApp.Models;

namespace ShoppingCartApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICartService _cartService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        public async Task<ServiceResponse> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return ServiceResponse.ShowView(
                    "Register",
                    new RegisterDto { FullName = dto.FullName, Email = dto.Email },
                    result.Errors.ToDictionary(_ => string.Empty, e => e.Description));

            await _signInManager.SignInAsync(user, isPersistent: false);

            return ServiceResponse.Redirect("Index", "Home");
        }

        public async Task<ServiceResponse> LoginAsync(LoginDto dto)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(
                dto.Email,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: false);

            if (!signInResult.Succeeded)
                return ServiceResponse.ShowView(
                    "Login",
                    new LoginDto { Email = dto.Email, RememberMe = dto.RememberMe },
                    string.Empty,
                    "Invalid email or password.");

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (!string.IsNullOrEmpty(dto.GuestId))
                await _cartService.MergeGuestCartAsync(dto.GuestId, user.Id);

            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Index",
                RedirectController = "Home",
                SessionRemoveKey = "GuestId"
            };
        }

        public async Task<ServiceResponse> LogoutAsync()
        {
            await _signInManager.SignOutAsync();

            return new ServiceResponse
            {
                Succeeded = true,
                RedirectAction = "Index",
                RedirectController = "Home",
                SessionRemoveKey = "__ALL__"
            };
        }

        public async Task<ServiceResponse> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return ServiceResponse.ShowView(
                    "NotFound", null, string.Empty, "User not found.");

            return ServiceResponse.ShowView("Profile", new ProfileDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address
            });
        }

        public async Task<ServiceResponse> UpdateProfileAsync(UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
                return ServiceResponse.ShowView(
                    "Profile", null, string.Empty, "User not found.");

            user.FullName = dto.FullName;
            user.Address = dto.Address;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return ServiceResponse.ShowView(
                    "Profile",
                    new UpdateProfileDto { FullName = dto.FullName, Address = dto.Address },
                    result.Errors.ToDictionary(_ => string.Empty, e => e.Description));

            return ServiceResponse.Redirect(
                "Profile",
                "Account",
                new Dictionary<string, string> { { "Success", "Profile updated successfully!" } });
        }

        public async Task<ServiceResponse> GeneratePasswordResetTokenAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // Always redirect — never reveal whether the email exists
            if (user == null)
                return ServiceResponse.Redirect("ForgotPasswordConfirmation", "Account");

            return ServiceResponse.Redirect(
                "ForgotPasswordConfirmation",
                "Account",
                new Dictionary<string, string> { { "ResetLink", dto.ResetUrl } });
        }

        public async Task<ServiceResponse> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return ServiceResponse.Redirect("ResetPasswordConfirmation", "Account");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.Password);

            if (!result.Succeeded)
                return ServiceResponse.ShowView(
                    "ResetPassword",
                    new ResetPasswordDto { Email = dto.Email, Token = dto.Token },
                    result.Errors.ToDictionary(_ => string.Empty, e => e.Description));

            return ServiceResponse.Redirect("ResetPasswordConfirmation", "Account");
        }
    }
}