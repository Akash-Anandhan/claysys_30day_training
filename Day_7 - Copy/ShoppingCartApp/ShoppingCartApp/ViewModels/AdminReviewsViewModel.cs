// ViewModels/AdminReviewsViewModel.cs
using ShoppingCartApp.Models;
using System.Collections.Generic;

namespace ShoppingCartApp.ViewModels
{
    public class AdminReviewsViewModel
    {
        // Search and Filter
        public string SearchQuery { get; set; }
        public int? MinRating { get; set; }
        public string SortBy { get; set; } // newest, oldest, rating_high, rating_low
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 50;
        
        // Data
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        
        // Computed
        public bool HasFilters => !string.IsNullOrEmpty(SearchQuery) || MinRating.HasValue || !string.IsNullOrEmpty(SortBy);
        public int StartItem => (CurrentPage - 1) * PageSize + 1;
        public int EndItem => System.Math.Min(CurrentPage * PageSize, TotalItems);
    }
}