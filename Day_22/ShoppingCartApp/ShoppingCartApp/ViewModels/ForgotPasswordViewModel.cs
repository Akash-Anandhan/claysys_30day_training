using System.ComponentModel.DataAnnotations;

namespace ShoppingCartApp.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
