using System.ComponentModel.DataAnnotations;

namespace ShoppingCartApp.ViewModels
{
    public class ReviewViewModel
    {
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5.")]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Review cannot exceed 500 characters.")]
        [Display(Name = "Your Review")]
        public string Comment { get; set; }
    }
}