using ShoppingCartApp.Models;
using System.Collections.Generic;

namespace ShoppingCartApp.ViewModels
{
    public class AdminProductsViewModel
    {
        public string SearchQuery { get; set; }
        public string Category { get; set; }
        public string StockFilter { get; set; } // all, in_stock, low_stock, out_of_stock
        public string SortBy { get; set; } // name_asc, name_desc, price_asc, price_desc, newest, stock_asc, stock_desc
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public int TotalResults { get; set; }
        public int TotalProducts { get; set; }
    }
}