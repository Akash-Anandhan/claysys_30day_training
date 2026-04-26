// DTOs/Auth/AuthDtos.cs
namespace ShoppingCartApp.DTOs.Auth
{

    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string GuestId { get; set; }  // passed in from session by controller
    }

    public class UpdateProfileDto
    {
        public string UserId { get; set; }  // resolved from ClaimsPrincipal by controller
        public string FullName { get; set; }
        public string Address { get; set; }
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; }
        public string ResetUrl { get; set; }  // built by Url.Action() in controller
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }

    public class ProfileDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}