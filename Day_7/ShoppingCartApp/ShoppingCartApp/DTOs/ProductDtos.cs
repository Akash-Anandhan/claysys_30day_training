// DTOs/Product/ProductDtos.cs
using ShoppingCartApp.Models;
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.DTOs.Product
{
    // ── Inbound DTOs (Controller → Service) ──

    public class ProductSearchDto
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; }
    }

    // ── Outbound DTOs (Service → Controller) ──

    public class ProductSearchResultDto
    {
        public IList<Models.Product> Results { get; set; }
        public IList<Category> Categories { get; set; }
        public int TotalResults { get; set; }
    }

    public class ProductListDto
    {
        public IList<Models.Product> Products { get; set; }
        public string PageTitle { get; set; }
        public string CategoryName { get; set; }
    }
}
