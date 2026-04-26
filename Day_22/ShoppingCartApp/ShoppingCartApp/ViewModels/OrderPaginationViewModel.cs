// ViewModels/OrderPaginationViewModel.cs
using ShoppingCartApp.Models;

namespace ShoppingCartApp.ViewModels
{
    public class OrderPaginationViewModel
    {
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public int TotalOrders { get; set; }
        public int TotalFilteredOrders { get; set; }
    }
}