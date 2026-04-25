// ViewModels/AdminOrdersViewModel.cs
using ShoppingCartApp.Models;
using System.Collections.Generic;

namespace ShoppingCartApp.ViewModels
{
    public class AdminOrdersViewModel
    {
        // Search and Filter
        public string SearchQuery { get; set; }
        public string StatusFilter { get; set; }
        public string SortBy { get; set; } // newest, oldest, amount_high, amount_low
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 50;
        
        // Data
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public Dictionary<string, string> UserEmails { get; set; } = new Dictionary<string, string>();
        
        // Computed
        public bool HasFilters => !string.IsNullOrEmpty(SearchQuery) || !string.IsNullOrEmpty(StatusFilter) || !string.IsNullOrEmpty(SortBy);
        public int StartItem => (CurrentPage - 1) * PageSize + 1;
        public int EndItem => System.Math.Min(CurrentPage * PageSize, TotalItems);
    }
}