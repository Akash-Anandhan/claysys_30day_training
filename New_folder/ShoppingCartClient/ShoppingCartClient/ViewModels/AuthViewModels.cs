using System.ComponentModel.DataAnnotations;

namespace ShoppingCartClient.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; } // UI only
    }

    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; } // UI only

        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public string Role { get; set; } = "User"; // default
    }
}