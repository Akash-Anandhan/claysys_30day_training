using ShoppingCartApp.Models;

namespace ShoppingCartApp.ViewModels
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; }
        public IEnumerable<Product> Results { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public int TotalResults { get; set; }
    }
}