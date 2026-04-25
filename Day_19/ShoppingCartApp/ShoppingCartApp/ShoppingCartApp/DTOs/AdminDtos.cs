// DTOs/Admin/AdminDtos.cs
using ShoppingCartApp.ViewModels;

namespace ShoppingCartApp.DTOs.Admin
{
    // ── Dashboard ──

    public class AdminDashboardDto
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalReviews { get; set; }
        public IList<Models.Order> RecentOrders { get; set; }
        public IList<Models.Product> LowStockProducts { get; set; }
    }

    // ── Products ──

    public class CreateProductDto
    {
        public ProductViewModel Model { get; set; }
        public string WebRootPath { get; set; }
    }

    public class EditProductDto
    {
        public int Id { get; set; }
        public ProductViewModel Model { get; set; }
        public string WebRootPath { get; set; }
    }

    // ── Orders ──

    public class AdminOrdersDto
    {
        public IList<Models.Order> Orders { get; set; }
        public Dictionary<string, string> UserEmails { get; set; }
    }

    public class AdminOrderDetailDto
    {
        public Models.Order Order { get; set; }
        public string UserEmail { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }

    // ── Import / Export ──

    public class ImportFileDto
    {
        public IFormFile File { get; set; }
    }
}
