// ViewModels/ProductPaginationViewModel.cs
using ShoppingCartApp.Models;

namespace ShoppingCartApp.ViewModels
{
    public class ProductPaginationViewModel
    {
        public string SearchQuery { get; set; }
        public string Category { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public int TotalProducts { get; set; }
    }
}