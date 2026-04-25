using System.ComponentModel.DataAnnotations;

namespace ShoppingCartApp.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Base Price is required.")]
        [Range(0.01, 99999.99, ErrorMessage = "Base Price must be greater than 0.")]
        [Display(Name = "Base Price (₹)")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Selling Price (₹)")]
        public decimal? SellingPrice { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, 9999, ErrorMessage = "Stock must be 0 or more.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Both image fields are fully optional
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
    }
}