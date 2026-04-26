using ShoppingCartApp.Models;
using System.Collections.Generic;

namespace ShoppingCartApp.ViewModels
{
    public class ProductPerformanceViewModel
    {
        public List<ProductPerformanceItem> Products { get; set; } = new();
        public int TotalProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUnitsSold { get; set; }
        public int TotalReviews { get; set; }
        
        // Filters
        public string SearchQuery { get; set; }
        public string Category { get; set; }
        public string SortBy { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        
        public List<Category> Categories { get; set; } = new();
    }

    public class ProductPerformanceItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        
        // Sales metrics
        public int TotalOrders { get; set; }
        public int TotalUnitsSold { get; set; }
        
        // Revenue metrics
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        
        // Review metrics
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        
        // Audience/Geography
        public Dictionary<string, int> CountryDistribution { get; set; } = new();
        public string TopCountry { get; set; }
        
        // Additional stats
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}