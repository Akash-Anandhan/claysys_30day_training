// ViewModels/AdminRevenueViewModel.cs
using ShoppingCartApp.DTOs.Admin;
using System.Collections.Generic;

namespace ShoppingCartApp.ViewModels
{
    public class AdminRevenueViewModel
    {
        // Filters
        public string DateRange { get; set; } = "This Year";
        public string SearchQuery { get; set; }
        
        // Data from dashboard
        public AdminDashboardDto Stats { get; set; }
        
        // Computed
        public bool HasFilters => !string.IsNullOrEmpty(SearchQuery);
    }
}